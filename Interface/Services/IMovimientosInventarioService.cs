using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;

namespace vet_api_Net.Interfaze.Services;

public interface IMovimientosInventarioService
{
    Task<MovimientoInventarioListResponseDTO> GetMovimientosAsync(
        int pageNumber,
        int pageSize,
        int? productId = null,
        string? movementType = null);
    Task RegisterMovementAsync(int productId, string movementType, int quantity, int previousStock, int newStock, string reason, int userId, int? referenceId = null);
    Task<List<ProductoStockBajoDTO>> GetLowStockAlertsAsync();
}
