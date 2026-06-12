using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaces.Repositories
{
    public interface IPetsRepository
    {
        Task<List<Mascota>> GetAllMascotasWithClienteAsync();
        Task<Mascota?> GetMascotaByIdWithClienteAsync(int id);
        Task<List<Mascota>> GetMascotasByClienteNameAsync(string nombre);
    }
}
