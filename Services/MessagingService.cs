using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTOs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using vet_api_Net.Constants;
using vet_api_Net.Data;
using vet_api_Net.Hubs;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Models;

//Describe: Servicio de mensajería interna entre usuarios de la clínica, con integración para respuestas automáticas del asistente IA de Groq.
namespace vet_api_Net.Services;

public class MessagingService : IMessagingService
{
    private readonly AppDbContext _context;
    private readonly IHubContext<MessageHub> _hubContext;
    private readonly IServiceScopeFactory _scopeFactory;

    public MessagingService(AppDbContext context, IHubContext<MessageHub> hubContext, IServiceScopeFactory scopeFactory)
    {
        _context = context;
        _hubContext = hubContext;
        _scopeFactory = scopeFactory;
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

        // Interceptar mensajes dirigidos al asistente Groq
        var receptor = await _context.Usuarios.FindAsync(dto.ReceptorId);
        if (receptor != null && receptor.Rol == "assistant" && receptor.Email == "groq@happy-pets.dev")
        {
            // Ejecutar la respuesta de la IA en segundo plano para no bloquear el envío
            _ = Task.Run(async () =>
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var groqService = scope.ServiceProvider.GetRequiredService<IGroqService>();
                var hub = scope.ServiceProvider.GetRequiredService<IHubContext<MessageHub>>();

                try
                {
                    // Obtener historial reciente para dar memoria al asistente
                    var conversationHistory = await db.Mensajes
                        .Where(m => (m.EmisorId == dto.EmisorId && m.ReceptorId == dto.ReceptorId) ||
                                    (m.EmisorId == dto.ReceptorId && m.ReceptorId == dto.EmisorId))
                        .OrderBy(m => m.FechaEnvio)
                        .Take(15)
                        .ToListAsync();

                    var sb = new System.Text.StringBuilder();
                    sb.AppendLine("Aquí está el historial reciente de nuestra conversación:");
                    foreach (var msg in conversationHistory)
                    {
                        var senderName = msg.EmisorId == dto.EmisorId ? "Usuario" : "Asistente";
                        sb.AppendLine($"{senderName}: {msg.Contenido}");
                    }
                    sb.AppendLine($"Usuario: {dto.Contenido}");

                    // Consultar a Groq
                    var groqResponse = await groqService.EnviarConsultaAsync(new GroqChatRequestDTO
                    {
                        Pregunta = sb.ToString()
                    });

                    // Guardar respuesta del asistente
                    var aiMsg = new Mensaje
                    {
                        EmisorId = dto.ReceptorId, // Groq es el emisor
                        ReceptorId = dto.EmisorId, // El usuario es el receptor
                        Contenido = groqResponse.Respuesta,
                        Leido = false,
                        FechaEnvio = DateTime.Now
                    };

                    db.Mensajes.Add(aiMsg);
                    await db.SaveChangesAsync();

                    var aiDto = new MensajeDTO
                    {
                        Id = aiMsg.Id,
                        EmisorId = aiMsg.EmisorId,
                        ReceptorId = aiMsg.ReceptorId,
                        Contenido = aiMsg.Contenido,
                        Leido = false,
                        FechaEnvio = aiMsg.FechaEnvio
                    };

                    // Notificar respuesta por SignalR
                    await hub.Clients.Group(aiMsg.ReceptorId.ToString()).SendAsync("ReceiveMessage", aiDto);
                    await hub.Clients.Group(aiMsg.EmisorId.ToString()).SendAsync("ReceiveMessage", aiDto);
                }
                catch (Exception ex)
                {
                    // Enviar mensaje de error en el chat si falla Groq
                    var errorMsg = new Mensaje
                    {
                        EmisorId = dto.ReceptorId,
                        ReceptorId = dto.EmisorId,
                        Contenido = "Lo siento, ocurrió un error al procesar tu mensaje con la IA: " + ex.Message,
                        Leido = false,
                        FechaEnvio = DateTime.Now
                    };
                    db.Mensajes.Add(errorMsg);
                    await db.SaveChangesAsync();

                    var errDto = new MensajeDTO
                    {
                        Id = errorMsg.Id,
                        EmisorId = errorMsg.EmisorId,
                        ReceptorId = errorMsg.ReceptorId,
                        Contenido = errorMsg.Contenido,
                        Leido = false,
                        FechaEnvio = errorMsg.FechaEnvio
                    };
                    await hub.Clients.Group(errorMsg.ReceptorId.ToString()).SendAsync("ReceiveMessage", errDto);
                }
            });
        }

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
