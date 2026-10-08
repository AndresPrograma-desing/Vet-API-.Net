using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;

namespace vet_api_Net.Interfaze.Services;

public interface IPetsService
{
    Task<MascotaListResponseDTO> GetAllMascotasAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null);
    Task<MascotaResumenDTO?> GetMascotaByIdAsync(int id);
    Task<MascotaResumenDTO?> UpdateMascotaAsync(int id, UpdatePetDTO dto);
    Task<bool> DeleteMascotaAsync(int id);
}
