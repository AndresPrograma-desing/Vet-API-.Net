using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Repositories;

public interface IPasswordRecoveryRepository
{
    Task<Usuario?> GetUserByEmailAsync(string email);
    Task<Usuario?> GetUserByRecoveryCodeAsync(string code);
    Task<EmailTemplate?> GetTemplateByTypeAsync(string typeEmail);
    Task AddTemplateAsync(EmailTemplate template);
    void UpdateUser(Usuario usuario);
    Task<bool> SaveChangesAsync();
}
