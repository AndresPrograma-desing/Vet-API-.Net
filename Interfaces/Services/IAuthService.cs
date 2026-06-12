using System.Threading.Tasks;
using vet_api_Net.Models;
using vet_api_Net.DTOs;

namespace vet_api_Net.Interfaces.Services;

public interface IAuthService
{
    Task<Usuario?> LoginAsync(LoginRequest request);
    string GenerateToken(Usuario user);
    Task<Usuario?> UpdatePasswordAsync(UpdatePasswordRequest request);
    Task<Usuario?> GetUserByIdAsync(int id);
}