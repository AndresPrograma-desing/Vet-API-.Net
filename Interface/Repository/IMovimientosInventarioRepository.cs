using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Repositories;

public interface IMovimientosInventarioRepository
{
    Task<(List<MovimientosInventario> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        int? productId = null,
        string? movementType = null);
    Task<List<Producto>> GetProductsWithLowStockAsync();
    void AddMovement(MovimientosInventario movement);
    Task<bool> SaveChangesAsync();
}
