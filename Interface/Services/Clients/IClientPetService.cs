using System.Threading.Tasks;
using DTOs;
using Microsoft.AspNetCore.Http;

namespace vet_api_Net.Interfaze.Services.Clients;

public interface IClientPetService
{
    Task<CreateClientWithPetResponseDTO> CreateClientWithPetAsync(CreateClientWithPetDTO dto);
    Task<CreatePetForExistingClientResponseDTO> CreatePetForExistingClientAsync(CreatePetForExistingClientDTO dto);
    Task<IEnumerable<ClientLookupResponseDTO>> GetClientsWithPetsLookupAsync(string searchTerm);
    Task<string> UploadClientImageAsync(int clientId, IFormFile file);
    Task<string> UploadPetImageAsync(int petId, IFormFile file);
}