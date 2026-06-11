using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.DTOs;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Repositories;

public interface IAlertsRepository
{
    Task<List<AlertNotificationDTO>> GetByDestinationAsync(string destination);
    Task<bool> AnyRecentDuplicateAsync(string title, System.DateTime threshold);
    void Add(AlertasInterna alert);
    Task<bool> SaveChangesAsync();
    Task<bool> MarkAsReadAsync(int alertId);
    Task<int> ExecutePurgeReadAlertsAsync();
}