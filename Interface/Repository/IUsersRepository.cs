using System.Threading.Tasks;
using vet_api_Net.Models;
using DTOs;

namespace vet_api_Net.Interfaze.Repositories;

public interface IUsersRepository
{
    Task<Usuario?> GetByEmailAndRolAsync(string email, string rol);
    Task<Usuario?> GetByEmailAsync(string email);
    Task<Usuario?> GetByNameAndApellidoAsync(string nombre, string apellido);
    Task<Usuario?> GetByIdAsync(int id);
    void Update(Usuario usuario);
    Task<bool> SaveChangesAsync();
    Task<bool> IsUserDisabledAsync(string email);
    Task<string?> GetRoleByEmailAsync(string email);
    Task<List<Usuario>> GetAllUsersAsync();
    Task<List<Usuario>> GetUsersByRoleAsync(string role);
    Task AddUserAsync(Usuario user);
    Task DeleteUserAsync(Usuario user);
    Task<ChangeNameUsersDTO?> ChangeNameUsersAsync(int id, string newName, string newLastName, string newEmail, string newPhone);
    Task<Usuario?> SaveAvatarUrl(int userId, string avatarUrl);
    Task<RolesRequestDTO> GetRolesAsync();

}