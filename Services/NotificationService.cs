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
            Titulo = dto.Titulo,
            Mensaje = dto.Mensaje,
            Tipo = dto.Tipo,
            Prioridad = dto.Prioridad,
            DestinatarioRol = dto.Destino,
            DestinatarioId = creatorId, // Almacena quién creó el ticket para mandarle la notificación cuando sea leído
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
    // 1. Ejecuta la actualización del estado persistente en la DB
    bool success = await _alertsRepository.MarkAsReadAsync(alertId);
    
    if (success)
    {
        // 2. Buscamos los datos para armar el mensaje y deducir el destino
        var user = await _usersRepository.GetByIdAsync(userId); // El usuario que hace la acción (ej: Veterinario)
        var alert = await _alertsRepository.GetByIdAsync(alertId); // Cargamos la alerta de la DB

        if (user != null && alert != null)
        {
            // AUTO-DESCUBRIMIENTO: Extraemos el id del destinatario original del ticket
            int targetUserId = alert.DestinatarioId ?? 0; 
            
            if (targetUserId > 0)
            {
                // Reemplazamos las variables dinámicas del mensaje de tu constante
                string message = ResponseMessagesNotificationPush.PushTicket.Replace("${user_name}", user.Nombre);
                
                // DISPARO PUSH: Llama a SignalR en memoria sin afectar la velocidad de la petición
                await _pushService.SendPushToUserAsync(targetUserId, message, alertId);
            }
        }
    }
    
    return success;
}
}