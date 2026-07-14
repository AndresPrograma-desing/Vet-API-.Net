using System.Threading.Tasks;
using DTOs;

namespace vet_api_Net.Interfaze.Services;

public interface IDashboardService
{
    Task<DashboardStatsDTO> GetDashboardStatsAsync(DateTime? startDate, DateTime? endDate, bool useUsd);
}
