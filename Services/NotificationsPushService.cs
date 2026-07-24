using System;
using System.Threading.Tasks;
using DTOs;
using Microsoft.AspNetCore.SignalR;
using vet_api_Net.Constants;
using vet_api_Net.Hubs;
using vet_api_Net.Interfaze.Services;

//Describe: Servicio para despachar notificaciones push a través de SignalR Hub a usuarios individuales o grupos por rol
namespace vet_api_Net.Services;

public class NotificationsPushService : INotificationsPushService
{
    private readonly IHubContext<NotificactionsPush> _hubContext;

    public NotificationsPushService(IHubContext<NotificactionsPush> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendPushToUserAsync(int targetUserId, string message, int alertId)
    {
        var data = new PushNotificationDTO
        {
            AlertId = alertId,
            Message = message,
            Title = "Ticket Visto",
            Type = "Ticket",
            UserId = targetUserId.ToString()
        };
        await _hubContext.Clients.Group(targetUserId.ToString()).SendAsync("ReceiveAlertUpdate", data);
    }

    public async Task SendNotificationAsync(PushNotificationDTO data)
    {
        if (string.IsNullOrWhiteSpace(data.UserId) || data.UserId.Equals(ResponseMessageNotificactionPush.AllUsers, StringComparison.OrdinalIgnoreCase))
        { 
            await _hubContext.Clients.All.SendAsync("ReceiveAlertUpdate", data);
        }
        else 
        {
            string target = data.UserId.Trim().ToLower();
            if (target == "admin" || target == "administrador")
            {
                await _hubContext.Clients.Groups("admin", "administrador").SendAsync("ReceiveAlertUpdate", data);
            }
            else if (target == "secretaria" || target == "secre")
            {
                await _hubContext.Clients.Groups("secretaria", "secre").SendAsync("ReceiveAlertUpdate", data);
            }
            else if (target == "doctor" || target == "doc" || target == "veterinario")
            {
                await _hubContext.Clients.Groups("doctor", "doc", "veterinario").SendAsync("ReceiveAlertUpdate", data);
            }
            else
            {
                await _hubContext.Clients.Group(target).SendAsync("ReceiveAlertUpdate", data);
            }
        }
    }
}