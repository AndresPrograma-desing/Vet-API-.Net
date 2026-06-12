using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Models;
using vet_api_Net.Interfaces.Repositories;

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

    public async Task<List<Cliente>> GetAllWithDetailsAsync()
    {
        return await _context.Clientes
            .Include(c => c.Mascota)
                .ThenInclude(m => m.Consulta)
            .Include(c => c.Mascota)
                .ThenInclude(m => m.Cita)
            .ToListAsync();
    }
}