using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using vet_api_Net.Interfaze.Repositories;

namespace vet_api_Net.Hubs;

public class NotificactionsPush : Hub
{
    private readonly IUsersRepository _usersRepository;

    public NotificactionsPush(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var userIdStr = httpContext?.Request.Query["userId"].FirstOrDefault();
        if (int.TryParse(userIdStr, out int userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
            
            var user = await _usersRepository.GetByIdAsync(userId);
            if (user != null && !string.IsNullOrEmpty(user.Rol))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, user.Rol.ToLower());
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
        }

        await base.OnDisconnectedAsync(exception);
    }
}
