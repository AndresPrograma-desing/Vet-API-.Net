using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Repositories;

public interface IWorkerConfigRepository
{
    Task<WorkerConfig?> GetByWorkerNameAsync(string workerName);
    Task<List<WorkerConfig>> GetAllAsync();
    void AddWorkerConfig(WorkerConfig config);
    void UpdateWorkerConfig(WorkerConfig config);
    Task<bool> SaveChangesAsync();
}
