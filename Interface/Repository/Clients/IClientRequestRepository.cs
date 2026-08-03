using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Repositories.Clients;

public interface IClientRepository
{
    Task<Cliente?> GetByIdSimpleAsync(int id);
    Task<(List<Cliente> Items, int TotalCount)> GetAllWithDetailsAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null);
    Task<Cliente?> DeleteClientAsync(int id);
    Task<Cliente> UpdateClientAsync(Cliente client);
}