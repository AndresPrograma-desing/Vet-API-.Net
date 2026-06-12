using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaces.Services;

public interface IClientService
{
    Task<Cliente?> GetClientAsync(int id);
    Task<List<ClientListWithMascotasDTO>> GetAllClientsWithDetailsAsync();
}