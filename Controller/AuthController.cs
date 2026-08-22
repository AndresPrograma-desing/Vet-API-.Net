using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vet_api_Net.Constants;
using vet_api_Net.DTOs;
using vet_api_Net.Infrastructure.Configuration;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Routes;
using vet_api_Net.Exceptions;

namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserPermissionService _userPermissionService;
    private readonly TokenTemporalOptions _tokenTemporalOptions;


    public AuthController(IAuthService authService, IUserPermissionService userPermissionService, Microsoft.Extensions.Options.IOptions<TokenTemporalOptions> options)
    {
        _authService = authService;
        _userPermissionService = userPermissionService;
        _tokenTemporalOptions = options.Value;
    }

    [HttpPost(Endpoints.Auth.Login)]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var user = await _authService.LoginAsync(request);
            if (user == null)
            {
                return Unauthorized(new { error = ResponseMessagesAuthController.Unauthorized });
            }

            var token = _authService.GenerateToken(user);

            return Ok(new
            {
                success = true,
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    rol = user.Rol
                },
                token
            });
        }
        catch (LoginSecurityException ex)
        {
            return BadRequest(new
            {
                error = ex.Security.Message,
                tryAgainTime = ex.Security.TryAgainTime
            });
        }
        catch (InvalidOperationException ex)
        {
            var error = ex;

            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet(Endpoints.Auth.Me)]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        try
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(idClaim) || !int.TryParse(idClaim, out var userId))
            {
                return Unauthorized(new { error = ResponseMessagesAuthController.InvalidToken });
            }

            var user = await _authService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized(new { error = ResponseMessagesUsers.UserNotFound });
            }

            var name = string.IsNullOrWhiteSpace(user.Nombre) && string.IsNullOrWhiteSpace(user.Apellido)
                ? string.Empty
                : $"{user.Nombre} {user.Apellido}".Trim();

            var permissions = await _userPermissionService.GetPermissionsAsync(user.Id);

            return Ok(new
            {
                id = user.Id.ToString(),
                name,
                email = user.Email,
                phone = user.Telefono,
                rol = user.Rol,
                avatar = user.AvatarUrl,
                doctor_id = (int?)null,
                permissions
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost(Endpoints.Auth.UpdatePassword)]
    [Authorize]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
    {
        try
        {
            var user = await _authService.UpdatePasswordAsync(request);
            if (user == null)
            {
                return NotFound(new { error = ResponseMessagesUsers.UserNotFound });
            }

            return Ok(new
            {
                success = true,
                message = ResponseMessagesAuthController.UpdatePasswordSuccess
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost(Endpoints.Auth.TempToken)]
    [AllowAnonymous]
    public IActionResult GetTempToken([FromBody] TempTokenRequest request, [FromServices] Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
    {
        if (!env.IsDevelopment())
        {
            return Forbid();
        }

        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password) ||
            request.Email != _tokenTemporalOptions.Email || request.Password != _tokenTemporalOptions.Password)
        {
            return Unauthorized(new { error = ResponseMessagesLogin.ErrorCredential });
        }

        int tempId = _tokenTemporalOptions.Id;

        if (int.TryParse(_tokenTemporalOptions.Id.ToString(), out var idVal))
        {
            tempId = idVal;
        }

        var tempUser = new vet_api_Net.Models.Usuario
        {
            Id = tempId,
            Email = _tokenTemporalOptions.Email,
            Password = _tokenTemporalOptions.Password,
            Rol = _tokenTemporalOptions.Rol,
            Nombre = _tokenTemporalOptions.Nombre,
            Apellido = _tokenTemporalOptions.Apellido
        };

        var token = _authService.GenerateToken(tempUser);
        return Ok(new
        {
            success = true,
            token,
            user = new
            {
                id = tempUser.Id,
                email = tempUser.Email,
                rol = tempUser.Rol
            }
        });
    }
}

// Clase encargada de tener el modelo de datos de la peticion de autenticacion de usuario 
public class TempTokenRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}