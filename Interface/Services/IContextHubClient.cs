using System.Threading.Tasks;
using DTOs;

namespace vet_api_Net.Interfaze.Services;

public interface IContextHubClient
{
    Task<ContextHubVerifyResponseDTO?> VerifyTicketAsync(string ticket);
    Task<ContextHubCreateUserResponseDTO?> CreateUserAsync(string email, string password);
}
