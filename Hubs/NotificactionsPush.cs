using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using vet_api_Net.Interfaze.Repositories;
using DTOs;

namespace vet_api_Net.Hubs;

public class NotificactionsPush : Hub
{
    private readonly IUsersRepository _usersRepository;
    private readonly vet_api_Net.Infrastructure.Configuration.TokenTemporalOptions _temporalToken;

    public NotificactionsPush(IUsersRepository usersRepository, Microsoft.Extensions.Options.IOptions<vet_api_Net.Infrastructure.Configuration.TokenTemporalOptions> temporalTokenOptions)
    {
        _usersRepository = usersRepository;
        _temporalToken = temporalTokenOptions.Value;
    }
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var userIdStr = httpContext?.Request.Query["userId"].FirstOrDefault();
        if (int.TryParse(userIdStr, out int userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
            
            var user = await _usersRepository.GetByIdAsync(userId);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(user.Rol))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, user.Rol.ToLower());
                }

                if (user.PasswordRecoveryCode == vet_api_Net.Constants.ResponseMessagesPasswordRecovery.RequirePasswordChangeCode)
                {
                    var pushNotification = new PushNotificationDTO
                    {
                        UserId = user.Id.ToString(),
                        Title = vet_api_Net.Constants.ResponseMessagesPasswordRecovery.PushNotificationChangePasswordTitle,
                        Message = vet_api_Net.Constants.ResponseMessagesPasswordRecovery.PushNotificationChangePasswordMessage,
                        Type = "SecurityAlert",
                        AlertId = 0
                    };
                    await Clients.Caller.SendAsync("ReceiveAlertUpdate", pushNotification);
                }
            }
            else if (userId == _temporalToken.Id)
            {
                if (!string.IsNullOrEmpty(_temporalToken.Rol))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, _temporalToken.Rol.ToLower());
                }
            }
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var httpContext = Context.GetHttpContext();
        var userIdStr = httpContext?.Request.Query["userId"].FirstOrDefault();
        if (int.TryParse(userIdStr, out int userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.ToString());
            
            var user = await _usersRepository.GetByIdAsync(userId);
            if (user != null && !string.IsNullOrEmpty(user.Rol))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, user.Rol.ToLower());
            }
            else if (userId == _temporalToken.Id && !string.IsNullOrEmpty(_temporalToken.Rol))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, _temporalToken.Rol.ToLower());
            }
        }

        await base.OnDisconnectedAsync(exception);
    }
}
