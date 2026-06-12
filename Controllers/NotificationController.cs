using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using vet_api_Net.Constants;
using vet_api_Net.Routes;
using vet_api_Net.DTOs;
using vet_api_Net.Interfaces.Services;

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

   [HttpGet("user/{userId}")]
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
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ResponseMessagesNotification.ErrorGettingNotifications, details = ex.Message });
        }
    }
    [HttpPost(Endpoints.NotificationController.Create)]
public async Task<ActionResult<AlertNotificationDTO>> Create([FromBody] AlertNotificationDTO notificationDto)
{
    try
    {
        var createdNotification = await _notificationService.CreateNotificationAsync(notificationDto);
        
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
    catch (Exception ex)
    {
        return StatusCode(500, new { message = ResponseMessagesNotification.ErrorCreatingNotification, details = ex.Message });
    }
}
    [HttpPost(Endpoints.NotificationController.MarkAsRead)]
    public async Task<ActionResult> MarkAsRead([FromRoute] int alertId)
    {
        try
        {
            var success = await _notificationService.MarkAsReadAsync(alertId);
            if (!success)
                return NotFound(new { message = ResponseMessagesNotification.NotificationNotFound });

            return Ok(new { message = ResponseMessagesNotification.NotificationMarkedAsRead });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ResponseMessagesNotification.ErrorUpdatingNotification, details = ex.Message });
        }
    }
}