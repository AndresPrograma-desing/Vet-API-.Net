using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Models;
using vet_api_Net.Interfaze.Repositories;

namespace vet_api_Net.Repositories;

public class PasswordRecoveryRepository : IPasswordRecoveryRepository
{
    private readonly AppDbContext _context;

    public PasswordRecoveryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> GetUserByEmailOrUsernameAsync(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier)) return null;

        var cleanIdentifier = identifier.Trim().ToLower();

        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email.ToLower() == cleanIdentifier
                                   || u.Nombre.ToLower() == cleanIdentifier);
    }
    
    public async Task<Usuario?> GetUserByRecoveryCodeAsync(string code)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.PasswordRecoveryCode == code);
    }

    public async Task<EmailTemplate?> GetTemplateByTypeAsync(string typeEmail)
    {
        return await _context.EmailTemplates.FirstOrDefaultAsync(t => t.TypeEmail == typeEmail);
    }

    public async Task AddTemplateAsync(EmailTemplate template)
    {
        await _context.EmailTemplates.AddAsync(template);
    }

    public void UpdateUser(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
