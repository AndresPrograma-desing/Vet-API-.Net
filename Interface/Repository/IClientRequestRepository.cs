using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Repositories;

public interface IClientRepository
{
    Task<Cliente?> GetByIdSimpleAsync(int id);
    Task<List<Cliente>> GetAllWithDetailsAsync();
    Task<Cliente?> DeleteClientAsync(int id);
    Task<Cliente> UpdateClientAsync(Cliente client);
}