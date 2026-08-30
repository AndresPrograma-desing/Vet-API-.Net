using System;
using System.Collections.Generic;
using System.Security.Claims; // Añadido para ClaimTypes
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using vet_api_Net.Constants;
using vet_api_Net.DTOs;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Routes;

namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet(Endpoints.NotificationController.NotificactionByUser)]
    public async Task<ActionResult<IEnumerable<AlertNotificationDTO>>> GetByUserId([FromRoute] int userId)
    {
        try
        {
            var result = await _notificationService.GetNotificationsByUserIdAsync(userId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = ResponseMessagesNotification.ErrorGettingNotifications });
        }
    }

    [HttpPost(Endpoints.NotificationController.Create)]
    public async Task<ActionResult<AlertNotificationDTO>> Create([FromBody] AlertNotificationDTO notificationDto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado o sesión inválida." });
            }

            var createdNotification = await _notificationService.CreateNotificationAsync(notificationDto, userId);
            return StatusCode(201, createdNotification);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = ResponseMessagesNotification.ErrorCreatingNotification });
        }
    }

    // --- ENDPOINT REFRACTORIZADO Y SEGURO ---
    [HttpPost(Endpoints.NotificationController.MarkAsRead)]
    public async Task<ActionResult> MarkAsRead([FromRoute] int alertId)
    {
        try
        {
            // 1. Extraer el ID del usuario del Token de manera limpia usando ClaimTypes.NameIdentifier
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (!int.TryParse(userIdClaim, out int userId))
            {
                // Si el token expiró, no es válido o está ausente
                return Unauthorized(new { message = "Usuario no autenticado o sesión inválida." });
            }

            // 2. Invocar el servicio único (este cambia el estado en DB y por dentro dispara SignalR de forma push)
            var success = await _notificationService.MarkAsReadAsync(alertId, userId);
            
            if (!success)
            {
                // Si el alertId no pertenece a ninguna notificación en el sistema (404)
                return NotFound(new { message = ResponseMessagesNotification.NotificationNotFound });
            }

            // 3. Respuesta exitosa (200 OK)
            return Ok(new { message = ResponseMessagesNotification.NotificationMarkedAsRead });
        }
        catch (KeyNotFoundException ex)
        {
            // Por si el usuario del token ya no existe físicamente en la DB
            return NotFound(new { message = ex.Message });
        }
        catch (Exception)
        {
            // Cualquier fallo inesperado del servidor se va por un 500 controlado
            return StatusCode(500, new { message = ResponseMessagesNotification.ErrorUpdatingNotification });
        }
    }
}