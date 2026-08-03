using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using vet_api_Net.Data;
using vet_api_Net.Models;
using vet_api_Net.Interfaze.Repositories.Clients;

namespace vet_api_Net.Repositories;

public class ClientPetRepository : IClientPetRepository
{
    private readonly AppDbContext _context;

    public ClientPetRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Clientes.AnyAsync(c => c.Email == email);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }

    public void AddClient(Cliente client)
    {
        _context.Clientes.Add(client);
    }

    public void AddPet(Mascota pet)
    {
        _context.Mascotas.Add(pet);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
    public async Task<bool> ClientExistsAsync(int clientId)
    {
        return await _context.Clientes.AnyAsync(c => c.Id == clientId);
    }
    public async Task<IEnumerable<Cliente>> GetClientsWithPetsLookupAsync(string searchTerm)
{
    var term = searchTerm?.Trim().ToLower() ?? string.Empty;

    var query = _context.Clientes
        .Include(c => c.Mascota)
        .AsQueryable();

    if (!string.IsNullOrEmpty(term))
    {
        query = query.Where(c => 
            c.Nombre.ToLower().Contains(term) || 
            c.Apellido.ToLower().Contains(term) || 
            c.Identificacion!.Contains(term) || 
            c.Mascota.Any(m => m.Nombre.ToLower().Contains(term))
        );
    }

    return await query.Take(20).ToListAsync();
}
public async Task<bool> PetExistsByIdentificacionAsync(string identificacion)
{
    if (string.IsNullOrWhiteSpace(identificacion)) return false;
    
    return await _context.Mascotas
        .AnyAsync(m => m.IdenteficacionMascota == identificacion.Trim());
}
}