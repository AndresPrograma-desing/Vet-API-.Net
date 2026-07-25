using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.DTOs;
using vet_api_Net.Models;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Repositories;
using DTOs;

//Describe: Servicio para la gestión de alertas internas y despacho de notificaciones en base de datos y tiempo real
namespace vet_api_Net.Services;

public class NotificationService : INotificationService
{
    private readonly IAlertsRepository _alertsRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly INotificationsPushService _pushService;

    public NotificationService(IAlertsRepository alertsRepository, IUsersRepository usersRepository, INotificationsPushService pushService)
    {
        _alertsRepository = alertsRepository;
        _usersRepository = usersRepository;
        _pushService = pushService;
    }

    public async Task<IEnumerable<AlertNotificationDTO>> GetNotificationsByUserIdAsync(int userId)
    {
        var user = await _usersRepository.GetByIdAsync(userId);
        if (user == null) 
            throw new KeyNotFoundException(ResponseMessagesUsers.UserNotFound);

        var userRole = user.Rol ?? string.Empty;

        return await _alertsRepository.GetByDestinationAsync(userRole);
    }

    public async Task<AlertNotificationDTO> CreateNotificationAsync(AlertNotificationDTO dto, int creatorId)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (string.IsNullOrWhiteSpace(dto.Titulo))
            throw new ArgumentException(ResponseMessagesNotification.TitleRequired);

        var threshold = DateTime.Now.AddMinutes(-5);
        if (await _alertsRepository.AnyRecentDuplicateAsync(dto.Titulo, threshold))
            throw new InvalidOperationException(ResponseMessagesNotification.DuplicateNotification);

        var nuevaAlerta = new AlertasInterna
        {
            Titulo = dto.Titulo ?? string.Empty,
            Mensaje = dto.Mensaje ?? string.Empty,
            Tipo = dto.Tipo ?? string.Empty,
            Prioridad = dto.Prioridad ?? string.Empty,
            DestinatarioRol = dto.Destino,
            DestinatarioId = creatorId,  
            Completada = dto.Completada,
            ReferenciaTabla = dto.Origen,
            Leida = dto.Leida,
            FechaLectura = dto.FechaLectura,
            CreatedAt = DateTime.Now
        };

        _alertsRepository.Add(nuevaAlerta);
        await _alertsRepository.SaveChangesAsync();

        try
        {
            var pushData = new PushNotificationDTO
            {
                AlertId = nuevaAlerta.Id,
                Title = nuevaAlerta.Titulo,
                Message = nuevaAlerta.Mensaje,
                Type = nuevaAlerta.Tipo ?? "Info",
                UserId = nuevaAlerta.DestinatarioRol ?? string.Empty
            };
            await _pushService.SendNotificationAsync(pushData);
        }
        catch (Exception)
        {
        }

        return dto;
    }
   public async Task<bool> MarkAsReadAsync(int alertId, int userId)
{
    bool success = await _alertsRepository.MarkAsReadAsync(alertId);
    
    if (success)
    {
        var user = await _usersRepository.GetByIdAsync(userId); 
        var alert = await _alertsRepository.GetByIdAsync(alertId);  

        if (user != null && alert != null)
        { 
            int targetUserId = alert.DestinatarioId ?? 0; 
            
            if (targetUserId > 0 && targetUserId != userId)
            { 
                string message = ResponseMessagesNotificationPush.PushTicket.Replace("${user_name}", user.Nombre);

                var readNoticeAlert = new AlertasInterna
                {
                    Titulo = "Ticket Visto",
                    Mensaje = message,
                    Tipo = "TicketVisto",
                    Prioridad = alert.Prioridad ?? "0",
                    DestinatarioRol = targetUserId.ToString(),
                    DestinatarioId = targetUserId,
                    Completada = false,
                    ReferenciaTabla = alertId.ToString(),
                    Leida = false,
                    CreatedAt = DateTime.Now
                };

                _alertsRepository.Add(readNoticeAlert);
                await _alertsRepository.SaveChangesAsync();

                await _pushService.SendPushToUserAsync(targetUserId, message, alertId);
            }
        }
    }
    
    return success;
}
}