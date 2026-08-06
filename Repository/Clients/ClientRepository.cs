using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Interfaze.Repositories.Clients;
using vet_api_Net.Models;

namespace vet_api_Net.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly AppDbContext _context;

    public ClientRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Cliente?> GetByIdSimpleAsync(int id)
    {
        return await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<(List<Cliente> Items, int TotalCount)> GetAllWithDetailsAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
    {
        var query = _context.Clientes.AsNoTracking();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(c => c.Nombre.ToLower().Contains(term) ||
                                     c.Apellido.ToLower().Contains(term) ||
                                     c.Identificacion.ToLower().Contains(term) ||
                                     c.Telefono.ToLower().Contains(term) ||
                                     c.Mascota.Any(m => m.Nombre.ToLower().Contains(term)));
        }

        var totalCount = await query.CountAsync();

        var clientIds = await query
            .OrderBy(c => c.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => c.Id)
            .ToListAsync();

        var items = await _context.Clientes
            .Where(c => clientIds.Contains(c.Id))
            .Include(c => c.Mascota)
                .ThenInclude(m => m.Consulta)
            .Include(c => c.Mascota)
                .ThenInclude(m => m.Cita)
            .OrderBy(c => c.Id)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Cliente?> DeleteClientAsync(int id)
    {
        var client = await _context.Clientes.FindAsync(id);
        if (client == null) return null;
        _context.Clientes.Remove(client);
        await _context.SaveChangesAsync();
        return client;
    }
    public async Task<Cliente> UpdateClientAsync(Cliente client)
    {
        _context.Clientes.Update(client);
        await _context.SaveChangesAsync();
        return client;
    }
    public async Task<Cliente?> GetByIdentificacionOrEmailSimpleAsync(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            return null;

        var term = identifier.Trim().ToLower();

        return await _context.Clientes
            .AsNoTracking()
            .Include(c => c.Mascota)
            .FirstOrDefaultAsync(c =>
                c.Identificacion.ToLower() == term ||
                c.Email.ToLower() == term);
    }
}