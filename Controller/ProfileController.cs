using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Services;
using DTOs;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly ISupabaseService _storageService;

    public ProfileController(ISupabaseService storageService)
    {
        _storageService = storageService;
    }

    [HttpPost("update-profile")]
    public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto dto)
    {
        try
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(idClaim) || !int.TryParse(idClaim, out var currentUserId))
            {
                return Unauthorized(new { success = false, message = "Token inválido o usuario no identificado" });
            }

            int targetUserId = currentUserId;
            if (dto.UserId.HasValue && dto.UserId.Value > 0)
            {
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                if (userRole == "admin" || userRole == "doctor" || userRole == "secretaria")
                {
                    targetUserId = dto.UserId.Value;
                }
                else
                {
                    return Forbid();
                }
            }

            Console.WriteLine($"[ProfileController] update-profile targetUserId={targetUserId} avatarRecibido={(dto.Avatar != null ? $"{dto.Avatar.FileName} ({dto.Avatar.Length} bytes)" : "null")}");

            string? avatarUrl = null;

            if (dto.Avatar != null)
            {
                avatarUrl = await _storageService.UploadAvatarAsync(dto.Avatar, targetUserId);
                Console.WriteLine($"[ProfileController] avatarUrl guardado para targetUserId={targetUserId}: {avatarUrl}");
            }

            return Ok(new
            {
                success = true,
                message = "Perfil actualizado correctamente",
                data = new
                {
                    username = dto.Username,
                    avatar_url = avatarUrl
                }
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new
            {
                success = false,
                message = ResponseMessagesProfileController.ErrorUpdatingProfile
            });
        }
    }
    [HttpGet("get-profile")]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            string userId = "usuario_ejemplo_123";
            string? avatarUrl = await _storageService.GetUrlAvatar(userId);

            return Ok(new
            {
                success = true,
                message = "Perfil obtenido correctamente",
                data = new
                {
                    username = "usuario_ejemplo_123",
                    avatar_url = avatarUrl
                }
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new
            {
                success = false,
                message = ResponseErrors.InternalServerError
            });
        }
    }
}