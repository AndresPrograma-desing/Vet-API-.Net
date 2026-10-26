using System.Threading.Tasks;
using DTOs;

namespace vet_api_Net.Interfaze.Services;

public interface IServicioService
{
    Task<ServicioListResponseDTO> GetServiciosAsync(int pageNumber = 1, int pageSize = 10, bool? active = null, string? searchTerm = null);
    Task<ServicioDTO?> GetByIdAsync(int id);
    Task<ServicioDTO> CreateAsync(ServicioCreateDTO dto);
    Task<ServicioDTO> UpdateAsync(int id, ServicioUpdateDTO dto);
    Task DeleteAsync(int id);
}
