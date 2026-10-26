using System.Threading.Tasks;
using DTOs;

namespace vet_api_Net.Interfaze.Services;

public interface IHistorialPrecioService
{
    Task<HistorialPrecioListResponseDTO> GetHistorialAsync(
        int pageNumber,
        int pageSize,
        int? productId = null);
    Task RegisterPriceChangeAsync(int productId, decimal previousPrice, decimal newPrice, int userId, string? reason = null);
}
