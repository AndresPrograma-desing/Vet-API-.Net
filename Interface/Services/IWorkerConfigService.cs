using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;

namespace vet_api_Net.Interfaze.Services;

public interface IWorkerConfigService
{
    Task<List<WorkerConfigDTO>> GetAllAsync();
    Task<WorkerConfigDTO?> GetByWorkerNameAsync(string workerName);
    Task<WorkerConfigDTO> UpdateAsync(string workerName, UpdateWorkerConfigDTO data);
}
