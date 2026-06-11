using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Repositories;

public interface IUsersRepository
{
    Task<Usuario?> GetByEmailAndRolAsync(string email, string rol);
    Task<Usuario?> GetByEmailAsync(string email);
    Task<Usuario?> GetByIdAsync(int id);
    void Update(Usuario usuario);
    Task<bool> SaveChangesAsync();
    Task<bool> IsUserDisabledAsync(string email);
}