using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;
using vet_api_Net.ReportSettings;

namespace vet_api_Net.Interfaces.Services
{
    public interface IReportSystemService
    {
        Task<IEnumerable<Reporte>> GetAllAsync();
        Task<Reporte> GetByIdAsync(int id);
        Task<Reporte> CreateAsync(Reporte reporte);
        Task<bool> DeleteAsync(int id);
        Task<Reporte> GenerateFullSystemReportAsync(string generadoPor);
        Task<object?> IsEnabledAsync();
        Task UpdateRetentionDaysAsync(int days);
        Task ToggleAutoDeleteAsync(bool enable);
        Task ToggleAutoGenerateAsync(bool enable);
    }
}
