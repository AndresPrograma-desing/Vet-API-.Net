using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using vet_api_Net.Interfaze.Repositories;
using DTOs;

//Describe: Hub de SignalR para envío y recepción de notificaciones push en tiempo real asociando conexiones a grupos por ID de usuario y roles
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
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
                
                var user = await _usersRepository.GetByIdAsync(userId);
                if (user != null)
                {
                    await RegisterRoleGroupsAsync(Context.ConnectionId, user.Rol);

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
                    await RegisterRoleGroupsAsync(Context.ConnectionId, _temporalToken.Rol);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SignalR OnConnectedAsync error: {ex.Message}");
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
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.ToString());
                
                var user = await _usersRepository.GetByIdAsync(userId);
                if (user != null)
                {
                    await UnregisterRoleGroupsAsync(Context.ConnectionId, user.Rol);
                }
                else if (userId == _temporalToken.Id)
                {
                    await UnregisterRoleGroupsAsync(Context.ConnectionId, _temporalToken.Rol);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SignalR OnDisconnectedAsync error: {ex.Message}");
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    private async Task RegisterRoleGroupsAsync(string connectionId, string? role)
    {
        if (string.IsNullOrWhiteSpace(role)) return;

        var normalized = role.Trim().ToLower();
        await Groups.AddToGroupAsync(connectionId, normalized);

        if (normalized == "admin" || normalized == "administrador")
        {
            await Groups.AddToGroupAsync(connectionId, "admin");
            await Groups.AddToGroupAsync(connectionId, "administrador");
        }
        else if (normalized == "secretaria" || normalized == "secre")
        {
            await Groups.AddToGroupAsync(connectionId, "secretaria");
            await Groups.AddToGroupAsync(connectionId, "secre");
        }
        else if (normalized == "doctor" || normalized == "doc" || normalized == "veterinario")
        {
            await Groups.AddToGroupAsync(connectionId, "doctor");
            await Groups.AddToGroupAsync(connectionId, "doc");
            await Groups.AddToGroupAsync(connectionId, "veterinario");
        }
    }

    private async Task UnregisterRoleGroupsAsync(string connectionId, string? role)
    {
        if (string.IsNullOrWhiteSpace(role)) return;

        var normalized = role.Trim().ToLower();
        await Groups.RemoveFromGroupAsync(connectionId, normalized);

        if (normalized == "admin" || normalized == "administrador")
        {
            await Groups.RemoveFromGroupAsync(connectionId, "admin");
            await Groups.RemoveFromGroupAsync(connectionId, "administrador");
        }
        else if (normalized == "secretaria" || normalized == "secre")
        {
            await Groups.RemoveFromGroupAsync(connectionId, "secretaria");
            await Groups.RemoveFromGroupAsync(connectionId, "secre");
        }
        else if (normalized == "doctor" || normalized == "doc" || normalized == "veterinario")
        {
            await Groups.RemoveFromGroupAsync(connectionId, "doctor");
            await Groups.RemoveFromGroupAsync(connectionId, "doc");
            await Groups.RemoveFromGroupAsync(connectionId, "veterinario");
        }
    }
}
