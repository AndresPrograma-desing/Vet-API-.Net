using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using vet_api_Net.Models;

//Describe: Contrato de acceso a datos para los reportes del sistema y su instantánea de datos.
namespace vet_api_Net.Interfaze.Repositories;

public interface IReportRepository
{
    Task<List<Reporte>> GetAllAsync();
    Task<Reporte?> GetByIdAsync(int id);
    Task<Reporte?> GetByIdTrackedAsync(int id);
    Task<List<Reporte>> GetExpiredAsync(DateTime deadline, CancellationToken cancellationToken = default);
    Task<Reporte?> GetLastByGeneratorAsync(string generatedBy, CancellationToken cancellationToken = default);
    Task AddAsync(Reporte reporte);
    void Remove(Reporte reporte);
    void RemoveRange(IEnumerable<Reporte> reportes);
    Task<(List<object> Clientes, List<object> Mascotas, List<object> Productos, List<object> Facturas, List<object> Usuarios)> GetSystemSnapshotAsync();
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}
