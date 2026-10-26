using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Repositories;

public interface IHistorialPrecioRepository
{
    Task<(List<HistorialPrecio> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        int? productId = null);
    void AddHistory(HistorialPrecio history);
    Task<bool> SaveChangesAsync();
}
