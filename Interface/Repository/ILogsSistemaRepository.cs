using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Repositories;

public interface ILogsSistemaRepository
{
    Task AddLogAsync(LogsSistema log);
    Task<List<LogsSistema>> GetLogsAsync(int pageNumber, int pageSize);
    Task<bool> SaveChangesAsync();
}
