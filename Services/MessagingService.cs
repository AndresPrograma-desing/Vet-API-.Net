using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using DTOs;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Data;
using vet_api_Net.Models;
using vet_api_Net.Hubs;
using vet_api_Net.Constants;

namespace vet_api_Net.Services;

public class MessagingService : IMessagingService
{
    private readonly AppDbContext _context;
    private readonly IHubContext<MessageHub> _hubContext;

    public MessagingService(AppDbContext context, IHubContext<MessageHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task<MensajeDTO> SendMessageAsync(CreateMensajeDTO dto)
    {
        var mensaje = new Mensaje
        {
            EmisorId = dto.EmisorId,
            ReceptorId = dto.ReceptorId,
            Contenido = dto.Contenido,
            Leido = false,
            FechaEnvio = DateTime.Now
        };

        _context.Mensajes.Add(mensaje);
        await _context.SaveChangesAsync();

        var result = new MensajeDTO
        {
            Id = mensaje.Id,
            EmisorId = mensaje.EmisorId,
            ReceptorId = mensaje.ReceptorId,
            Contenido = mensaje.Contenido,
            Leido = mensaje.Leido ?? false,
            FechaEnvio = mensaje.FechaEnvio
        };
 
        await _hubContext.Clients.Group(mensaje.ReceptorId.ToString()).SendAsync("ReceiveMessage", result);
        await _hubContext.Clients.Group(mensaje.EmisorId.ToString()).SendAsync("ReceiveMessage", result);

        return result;
    }

    public async Task<List<MensajeDTO>> GetConversationAsync(int userId, int otherUserId)
    {
        var rows = await _context.Mensajes
            .Where(m => (m.EmisorId == userId && m.ReceptorId == otherUserId) || (m.EmisorId == otherUserId && m.ReceptorId == userId))
            .OrderBy(m => m.FechaEnvio)
            .ToListAsync();

        return rows.Select(m => new MensajeDTO
        {
            Id = m.Id,
            EmisorId = m.EmisorId,
            ReceptorId = m.ReceptorId,
            Contenido = m.Contenido,
            Leido = m.Leido ?? false,
            FechaEnvio = m.FechaEnvio
        }).ToList();
    }

    public async Task<List<MensajeDTO>> GetUserMessagesAsync(int userId)
    {
        var rows = await _context.Mensajes
            .Where(m => m.ReceptorId == userId)
            .OrderByDescending(m => m.FechaEnvio)
            .ToListAsync();

        return rows.Select(m => new MensajeDTO
        {
            Id = m.Id,
            EmisorId = m.EmisorId,
            ReceptorId = m.ReceptorId,
            Contenido = m.Contenido,
            Leido = m.Leido ?? false,
            FechaEnvio = m.FechaEnvio
        }).ToList();
    }

    public async Task MarkAsReadAsync(int messageId)
    {
        var msg = await _context.Mensajes.FindAsync(messageId);
        if (msg == null) throw new KeyNotFoundException(ResponseMessagesMessaging.MessageNotFound);

        msg.Leido = true;
        await _context.SaveChangesAsync();

        var dto = new MensajeDTO
        {
            Id = msg.Id,
            EmisorId = msg.EmisorId,
            ReceptorId = msg.ReceptorId,
            Contenido = msg.Contenido,
            Leido = true,
            FechaEnvio = msg.FechaEnvio
        };

        await _hubContext.Clients.Group(msg.EmisorId.ToString()).SendAsync("MessageRead", dto);
    }
}
