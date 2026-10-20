using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Services.Clients;

public interface IClientAuthService
{
    Task<bool> RequestLoginCodeAsync(string identifier);
    Task<Cliente?> VerifyLoginCodeAsync(string code);
    string GenerateToken(Cliente client);
}
