using System;
using System.Threading.Tasks;
using DTOs;
using Microsoft.AspNetCore.SignalR;
using vet_api_Net.Constants;
using vet_api_Net.Hubs;
using vet_api_Net.Interfaze.Services;

namespace vet_api_Net.Services;
/*
    SERVICIO DE NOTIFICACIONES PUSH AL FRONTEND.
        
*/
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
        else if (data.UserId.Equals(ResponseMessageNotificactionPush.Admin, StringComparison.OrdinalIgnoreCase) ||
                 data.UserId.Equals(ResponseMessageNotificactionPush.SE, StringComparison.OrdinalIgnoreCase) ||
                 data.UserId.Equals(ResponseMessageNotificactionPush.DC, StringComparison.OrdinalIgnoreCase))
        {
            string roleGroup = data.UserId.ToLower();
            await _hubContext.Clients.Group(roleGroup).SendAsync("ReceiveAlertUpdate", data);
        }
        else
        {
            await _hubContext.Clients.Group(data.UserId).SendAsync("ReceiveAlertUpdate", data);
        }
    }
}