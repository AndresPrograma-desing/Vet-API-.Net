using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Repositories;

public interface ILogsSistemaRepository
{
    Task AddLogAsync(LogsSistema log);
    Task<(List<LogsSistema> Items, int TotalCount)> GetLogsAsync(
        int pageNumber,
        int pageSize,
        int? userId = null,
        string? action = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null);
    Task<bool> SaveChangesAsync();
}
