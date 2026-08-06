using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Services.Clients;

public interface IClientService
{
    Task<Cliente?> GetClientAsync(int id);
    Task<(List<ClientListWithMascotasDTO> Items, int TotalCount)> GetAllClientsWithDetailsAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null);
    Task<Cliente?> DeleteClientAsync(int id);
    Task<Cliente?> UpdateClientAsync(int id, UpdateClientDTO dto);
    Task<ClientLookupResponseDTO?> GetByIdentificacionOrEmailAsync(string identifier);
}