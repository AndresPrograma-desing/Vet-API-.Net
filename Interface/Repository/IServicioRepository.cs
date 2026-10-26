using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Repositories;

public interface IServicioRepository
{
    Task<(List<Servicio> Items, int TotalCount)> GetPagedAsync(int pageNumber = 1, int pageSize = 10, bool? active = null, string? searchTerm = null);
    Task<Servicio?> GetByIdAsync(int id);
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    void AddServicio(Servicio servicio);
    void UpdateServicio(Servicio servicio);
    void DeleteServicio(Servicio servicio);
    Task<bool> SaveChangesAsync();
}
