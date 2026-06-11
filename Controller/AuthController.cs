using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using vet_api_Net.Constants;
using vet_api_Net.Routes;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.DTOs;

namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost(Endpoints.Auth.Login)]
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
        catch (InvalidOperationException ex)
        {
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

            return Ok(new
            {
                id = user.Id.ToString(),
                name,
                email = user.Email,
                rol = user.Rol,
                doctor_id = (int?)null
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
}