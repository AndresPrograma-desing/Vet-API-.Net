using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;

namespace vet_api_Net.Interfaces.Services;

public interface IPetsService
{
    Task<List<MascotaResumenDTO>> GetAllMascotasAsync();
    Task<MascotaResumenDTO?> GetMascotaByIdAsync(int id);
}
