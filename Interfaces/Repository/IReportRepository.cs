using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaces.Repositories
{
    public interface IReportRepository
    {
        Task<IEnumerable<Reporte>> GetAllAsync();
        Task<Reporte?> GetByIdAsync(int id);
        Task AddAsync(Reporte reporte);
        void Delete(Reporte reporte);
        Task<bool> SaveChangesAsync();
        Task<object?> GetSystemReportDataAsync();
        Task<ReporConfig?> GetConfigAsync();
        void AddConfig(ReporConfig config);
        void UpdateConfig(ReporConfig config);
    }
}
