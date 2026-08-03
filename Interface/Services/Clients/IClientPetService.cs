using System.Threading.Tasks;
using DTOs;

namespace vet_api_Net.Interfaze.Services.Clients;

public interface IClientPetService
{
    Task<CreateClientWithPetResponseDTO> CreateClientWithPetAsync(CreateClientWithPetDTO dto);
    Task<CreatePetForExistingClientResponseDTO> CreatePetForExistingClientAsync(CreatePetForExistingClientDTO dto);
    Task<IEnumerable<ClientLookupResponseDTO>> GetClientsWithPetsLookupAsync(string searchTerm);
}