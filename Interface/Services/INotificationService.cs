using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.DTOs;

namespace vet_api_Net.Interfaze.Services;

public interface INotificationService
{
    Task<IEnumerable<AlertNotificationDTO>> GetNotificationsByUserIdAsync(int userId);
    Task<AlertNotificationDTO> CreateNotificationAsync(AlertNotificationDTO dto, int? creatorId);
    Task<bool> MarkAsReadAsync(int alertId, int userId);
    Task<IEnumerable<AlertNotificationDTO>> GetNotificationsByClienteIdAsync(int clienteId);
    Task<bool> MarkClientAlertAsReadAsync(int alertId, int clienteId);
    Task RespondToCitaRequestAsync(int alertId, bool aceptar, string? mensaje);
}