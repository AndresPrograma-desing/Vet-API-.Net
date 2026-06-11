using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;

namespace vet_api_Net.Interfaze.Services;

public interface IUserPetsService
{
    Task<List<MascotaResumenDTO>> GetUserPetsAsync(string nombre);
}
