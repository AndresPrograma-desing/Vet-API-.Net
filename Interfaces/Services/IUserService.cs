using vet_api_Net.Data;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Models;
using DTOs;

namespace vet_api_Net.Interfaces.Services;

public interface IUserService
{
    Task<List<Usuario>> GetAllUserAsync();

    Task<Usuario> CreateUserAsync(CreateUserDTO userDto);
    Task<List<Usuario>> GetSecretariasAsync();

    Task<Usuario?> DisableUserAsync(int id);
    Task<Usuario?> EnableUserAsync(int id);

    Task<Usuario?> DeleteUserAsync(int id);

    Task<string?> UserStatusAsync(int id);
}