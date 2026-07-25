using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;

namespace vet_api_Net.Interfaze.Services;

public interface IPetsService
{
    Task<List<MascotaResumenDTO>> GetAllMascotasAsync();
    Task<MascotaResumenDTO?> GetMascotaByIdAsync(int id);
    Task<MascotaResumenDTO?> UpdateMascotaAsync(int id, UpdatePetDTO dto);
    Task<bool> DeleteMascotaAsync(int id);
}
