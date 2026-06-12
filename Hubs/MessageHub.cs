using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using DTOs;
using vet_api_Net.Interfaces.Services;

namespace vet_api_Net.Hubs;

public class MessageHub : Hub
{
    private readonly IMessagingService _messagingService;

    public MessageHub(IMessagingService messagingService)
    {
        _messagingService = messagingService;
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var userIdStr = httpContext?.Request.Query["userId"].FirstOrDefault();
        if (int.TryParse(userIdStr, out int userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
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
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMensajeDTO dto)
    {
        var sent = await _messagingService.SendMessageAsync(dto);
        await Clients.Caller.SendAsync("MessageSent", sent);
    }
}
