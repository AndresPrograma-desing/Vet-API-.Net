using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;

namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificactionPushController : ControllerBase
{
    private readonly INotificationsPushService _pushService;
    private readonly IUsersRepository _usersRepository;
    private readonly IAlertsRepository _alertsRepository;

    public NotificactionPushController(INotificationsPushService pushService, IUsersRepository usersRepository, IAlertsRepository alertsRepository)
    {
        _pushService = pushService;
        _usersRepository = usersRepository;
        _alertsRepository = alertsRepository;
    }

    [HttpPost("send/{alertId}")]
    public async Task<IActionResult> SendAlert([FromRoute] int alertId)
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado." });
            }

            var user = await _usersRepository.GetByIdAsync(userId);
            var alert = await _alertsRepository.GetByIdAsync(alertId);

            if (user == null || alert == null)
            {
                return NotFound(new { message = "Usuario o alerta no encontrada." });
            }

            int targetUserId = alert.DestinatarioId ?? 0;
            if (targetUserId == 0)
            {
                return BadRequest(new { message = "La alerta no tiene un destinatario específico asociado." });
            }

            string message = ResponseMessagesNotificationPush.PushTicket.Replace("${user_name}", user.Nombre);

            await _pushService.SendPushToUserAsync(targetUserId, message, alertId);

            return Ok(new { message = ResponseMessagesNotificationPush.Success });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ResponseMessagesNotificationPush.CriticalFailure, details = ex.Message });
        }
    }
}
