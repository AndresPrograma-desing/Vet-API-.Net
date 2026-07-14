using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.DTOs;
using vet_api_Net.Models;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Repositories;

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
            
            if (targetUserId > 0)
            { 
                string message = ResponseMessagesNotificationPush.PushTicket.Replace("${user_name}", user.Nombre);
                
                await _pushService.SendPushToUserAsync(targetUserId, message, alertId);
            }
        }
    }
    
    return success;
}
}