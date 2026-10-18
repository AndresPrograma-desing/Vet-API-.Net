using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Models;

//Describe: Implementación del repositorio de credenciales de integraciones externas, usado para la rotación de API keys por proveedor.
namespace vet_api_Net.Repositories;

public class IntegrationCredentialRepository : IIntegrationCredentialRepository
{
    private readonly AppDbContext _context;

    public IntegrationCredentialRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<IntegrationCredential>> GetActiveByProviderAsync(string provider)
        => await _context.IntegrationCredentials
            .Where(c => c.Provider == provider && c.IsActive)
            .OrderBy(c => c.Id)
            .ToListAsync();

    public async Task<List<IntegrationCredential>> GetAllAsync()
        => await _context.IntegrationCredentials
            .OrderBy(c => c.Provider)
            .ThenBy(c => c.Id)
            .ToListAsync();

    public async Task<IntegrationCredential?> GetByIdAsync(int id)
        => await _context.IntegrationCredentials.FindAsync(id);

    public void Add(IntegrationCredential credential)
        => _context.IntegrationCredentials.Add(credential);

    public void Update(IntegrationCredential credential)
        => _context.IntegrationCredentials.Update(credential);

    public void Delete(IntegrationCredential credential)
        => _context.IntegrationCredentials.Remove(credential);

    public async Task<bool> SaveChangesAsync()
        => await _context.SaveChangesAsync() > 0;
}
