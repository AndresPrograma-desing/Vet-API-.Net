using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTOs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using vet_api_Net.Constants;
using vet_api_Net.Hubs;
using vet_api_Net.Interface.Services;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Models;

//Describe: Servicio de mensajería interna entre usuarios de la clínica, con integración para respuestas automáticas del asistente IA de Groq.
namespace vet_api_Net.Services;

public class MessagingService : IMessagingService
{
    private readonly IMessagingRepository _repository;
    private readonly IUsersRepository _usersRepository;
    private readonly IHubContext<MessageHub> _hubContext;
    private readonly IServiceScopeFactory _scopeFactory;

    public MessagingService(IMessagingRepository repository, IUsersRepository usersRepository, IHubContext<MessageHub> hubContext, IServiceScopeFactory scopeFactory)
    {
        _repository = repository;
        _usersRepository = usersRepository;
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

        await _repository.AddAsync(mensaje);
        await _repository.SaveChangesAsync();

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
 
        var receptor = await _usersRepository.GetByIdAsync(dto.ReceptorId);
        if (receptor != null && receptor.Rol == "assistant" && receptor.Email == "groq@happy-pets.dev")
        {
            _ = Task.Run(async () =>
            {
                using var scope = _scopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IMessagingRepository>();
                var groqService = scope.ServiceProvider.GetRequiredService<INowGrodService>();
                var hub = scope.ServiceProvider.GetRequiredService<IHubContext<MessageHub>>();

                try
                {
                    var conversationHistory = await repository.GetRecentConversationAsync(dto.EmisorId, dto.ReceptorId, 15);

                    var sb = new System.Text.StringBuilder();
                    sb.AppendLine("Aquí está el historial reciente de nuestra conversación:");
                    foreach (var msg in conversationHistory)
                    {
                        var senderName = msg.EmisorId == dto.EmisorId ? "Usuario" : "Asistente";
                        sb.AppendLine($"{senderName}: {msg.Contenido}");
                    }
                    sb.AppendLine($"Usuario: {dto.Contenido}");

                    var groqResponse = await groqService.EnviarConsultaAsync(new GroqChatRequestDTO
                    {
                        Pregunta = sb.ToString()
                    });

                    var aiMsg = new Mensaje
                    {
                        EmisorId = dto.ReceptorId,  
                        ReceptorId = dto.EmisorId,  
                        Contenido = groqResponse.Respuesta,
                        Leido = false,
                        FechaEnvio = DateTime.Now
                    };

                    await repository.AddAsync(aiMsg);
                    await repository.SaveChangesAsync();

                    var aiDto = new MensajeDTO
                    {
                        Id = aiMsg.Id,
                        EmisorId = aiMsg.EmisorId,
                        ReceptorId = aiMsg.ReceptorId,
                        Contenido = aiMsg.Contenido,
                        Leido = false,
                        FechaEnvio = aiMsg.FechaEnvio
                    };
 
                    await hub.Clients.Group(aiMsg.ReceptorId.ToString()).SendAsync("ReceiveMessage", aiDto);
                    await hub.Clients.Group(aiMsg.EmisorId.ToString()).SendAsync("ReceiveMessage", aiDto);
                }
                catch (Exception ex)
                { 
                    var errorMsg = new Mensaje
                    {
                        EmisorId = dto.ReceptorId,
                        ReceptorId = dto.EmisorId,
                        Contenido = "Lo siento, ocurrió un error al procesar tu mensaje con la IA: " + ex.Message,
                        Leido = false,
                        FechaEnvio = DateTime.Now
                    };
                    await repository.AddAsync(errorMsg);
                    await repository.SaveChangesAsync();

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
        var rows = await _repository.GetConversationAsync(userId, otherUserId);

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
        var rows = await _repository.GetByReceptorAsync(userId);

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
        var msg = await _repository.GetByIdAsync(messageId);
        if (msg == null) throw new KeyNotFoundException(ResponseMessagesMessaging.MessageNotFound);

        msg.Leido = true;
        await _repository.SaveChangesAsync();

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
