using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Models;
using vet_api_Net.Interfaze.Repositories;

namespace vet_api_Net.Repositories;

public class EmailTemplateRepository : IEmailTemplateRepository
{
    private readonly AppDbContext _context;

    public EmailTemplateRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<EmailTemplate?> GetTemplateByTypeAsync(string typeEmail)
    {
        return await _context.EmailTemplates
            .FirstOrDefaultAsync(t => t.TypeEmail == typeEmail);
    }

    public async Task<System.Collections.Generic.IEnumerable<EmailTemplate>> GetAllTemplatesAsync()
    {
        return await _context.EmailTemplates.ToListAsync();
    }

    public async Task<EmailTemplate?> GetTemplateByIdAsync(int id)
    {
        return await _context.EmailTemplates.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<bool> UpdateTemplateAsync(EmailTemplate template)
    {
        _context.EmailTemplates.Update(template);
        return await _context.SaveChangesAsync() > 0;
    }
}
