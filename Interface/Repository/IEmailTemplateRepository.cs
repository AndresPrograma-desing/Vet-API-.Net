using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Repositories;

public interface IEmailTemplateRepository
{
    Task<EmailTemplate?> GetTemplateByTypeAsync(string typeEmail);
    Task<System.Collections.Generic.IEnumerable<EmailTemplate>> GetAllTemplatesAsync();
    Task<EmailTemplate?> GetTemplateByIdAsync(int id);
    Task<bool> UpdateTemplateAsync(EmailTemplate template);
}
