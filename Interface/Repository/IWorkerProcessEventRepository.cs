using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Repositories;

public interface IWorkerProcessEventRepository
{
    Task<WorkerProcessEvent?> GetActiveByNameAndVinculationAsync(string nameEvent, string? vinculationData);
    Task AddAsync(WorkerProcessEvent processEvent);
    Task<(List<WorkerProcessEvent> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? typeEvent,
        bool? isResolved,
        DateTime? fromDate,
        DateTime? toDate);
    Task<List<WorkerProcessEvent>> GetResolvedOlderThanAsync(DateTime threshold);
    void RemoveRange(IEnumerable<WorkerProcessEvent> processEvents);
    Task<bool> SaveChangesAsync();
}
