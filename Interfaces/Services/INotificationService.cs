using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.DTOs;

namespace vet_api_Net.Interfaces.Services;

public interface INotificationService
{
    Task<IEnumerable<AlertNotificationDTO>> GetNotificationsByUserIdAsync(int userId);
    Task<AlertNotificationDTO> CreateNotificationAsync(AlertNotificationDTO dto);
    Task<bool> MarkAsReadAsync(int alertId);
}