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

    [HttpPost(Endpoints.NotificationController.MarkAsRead)]
    public async Task<ActionResult> MarkAsRead([FromRoute] int alertId)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado o sesión inválida." });
            }

            var success = await _notificationService.MarkAsReadAsync(alertId, userId);
            
            if (!success)
            { 
                return NotFound(new { message = ResponseMessagesNotification.NotificationNotFound });
            }
 
            return Ok(new { message = ResponseMessagesNotification.NotificationMarkedAsRead });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception)
        { 
            return StatusCode(500, new { message = ResponseMessagesNotification.ErrorUpdatingNotification });
        }
    }

    [HttpPost(Endpoints.NotificationController.RespondCitaRequest)]
    public async Task<IActionResult> RespondCitaRequest([FromRoute] int alertId, [FromBody] RespondCitaRequestDTO request)
    {
        try
        {
            await _notificationService.RespondToCitaRequestAsync(alertId, request?.Aceptar ?? false, request?.Mensaje);
            return Ok(new { success = true, message = ResponseMessagesNotification.CitaRequestRespondedSuccess });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { success = false, error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { success = false, error = ResponseMessagesNotification.ErrorUpdatingNotification });
        }
    }
}