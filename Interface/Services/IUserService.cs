using DTOs;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Services;

public interface IUserService
{
    Task<List<Usuario>> GetAllUserAsync();

    Task<Usuario> CreateUserAsync(CreateUserDTO userDto);
    Task<List<Usuario>> GetSecretariasAsync();

    Task<Usuario?> DisableUserAsync(int id);
    Task<Usuario?> EnableUserAsync(int id);

    Task<Usuario?> DeleteUserAsync(int id);

    Task<string?> UserStatusAsync(int id);

    Task<bool> RequestPasswordResetAsync(string email, string resetLinkBase);
    Task<bool> ConfirmPasswordResetTicketAsync(string token);
    Task<ResetStatusResponseDTO?> GetResetStatusAsync(string email);
    Task<List<ResetStatusResponseDTO>> GetPendingPasswordResetsAsync();
    Task<bool> AssignNewPasswordAsync(string email, string newPassword);
    Task<string?> GetTemplatePasswordResetPage(string message);
    Task<ChangeNameUsersDTO?> ChangeNameUsersAsync(int id, string newName);
}