using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;
using DTOs;

namespace vet_api_Net.Interfaze.Repositories;

public interface IDashboardRepository
{
    Task<decimal> GetTotalGananciasAsync(DateTime? startDate, DateTime? endDate);
    Task<decimal> GetTotalPerdidasAsync(DateTime? startDate, DateTime? endDate);
    Task<int> GetTotalFacturasCountAsync(DateTime? startDate, DateTime? endDate);
    Task<int> GetTotalCitasAsync(DateTime? startDate, DateTime? endDate);
    Task<int> GetTotalMascotasAsync(DateTime? startDate, DateTime? endDate);
    Task<int> GetTotalClientesAsync(DateTime? startDate, DateTime? endDate);
    Task<int> GetTotalProductosAsync(DateTime? startDate, DateTime? endDate);
    Task<Dictionary<string, int>> GetCitasPorEstadoAsync(DateTime? startDate, DateTime? endDate);
    Task<List<Cita>> GetUltimasCitasAsync(int count);
    Task<List<Producto>> GetProductosBajoStockAsync(int count);
    Task<List<AlertasInterna>> GetAlertasRecientesAsync(int count);
    Task<List<DashboardEarningByMonthDTO>> GetGananciasMensualesAsync(int meses);
}
