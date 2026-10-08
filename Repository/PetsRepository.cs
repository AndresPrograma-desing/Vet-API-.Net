using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Extensions;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Models;

//Describe: Acceso a datos de mascotas, incluyendo la eliminación en cascada manual de sus registros dependientes.
namespace vet_api_Net.Repositories;

public class PetsRepository : IPetsRepository
{
    private readonly AppDbContext _context;

    public PetsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(List<Mascota> Items, int TotalCount)> GetPagedWithRelationsAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
    {
        var query = _context.Mascotas
            .Include(m => m.Cliente)
            .Include(m => m.Especie)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(m => m.Nombre.ToLower().Contains(term)
                || (m.Cliente != null && (m.Cliente.Nombre.ToLower().Contains(term) || m.Cliente.Apellido.ToLower().Contains(term))));
        }

        return await query
            .OrderByDescending(m => m.Id)
            .ToPagedResultAsync(pageNumber, pageSize);
    }

    public async Task<Mascota?> GetByIdWithRelationsAsync(int id)
        => await _context.Mascotas
            .Include(m => m.Cliente)
            .Include(m => m.Especie)
            .FirstOrDefaultAsync(m => m.Id == id);

    public async Task<List<Mascota>> GetByClientNameWithRelationsAsync(string lowerClientName)
        => await _context.Mascotas
            .Include(m => m.Cliente)
            .Include(m => m.Especie)
            .Where(m => m.Cliente != null && m.Cliente.Nombre.ToLower() == lowerClientName)
            .ToListAsync();

    public async Task<bool> ExistsAsync(int id)
        => await _context.Mascotas.AnyAsync(m => m.Id == id);

    public void Update(Mascota mascota)
        => _context.Mascotas.Update(mascota);

    public async Task<bool> DeleteWithDependenciesAsync(int id)
    {
        var consultaIds = await _context.Consultas
            .Where(c => c.MascotaId == id)
            .Select(c => c.Id)
            .ToListAsync();

        if (consultaIds.Any())
        {
            await _context.ConsultasProductos
                .Where(cp => consultaIds.Contains(cp.ConsultaId))
                .ExecuteDeleteAsync();
        }

        var facturaIds = await _context.Facturas
            .Where(f => f.MascotaId == id)
            .Select(f => f.Id)
            .ToListAsync();

        if (facturaIds.Any())
        {
            await _context.DetallesFacturas
                .Where(df => facturaIds.Contains(df.FacturaId))
                .ExecuteDeleteAsync();
        }

        await _context.Facturas
            .Where(f => f.MascotaId == id)
            .ExecuteDeleteAsync();

        await _context.PetVaccinations
            .Where(v => v.MascotaId == id)
            .ExecuteDeleteAsync();

        await _context.HistoriasClinicas
            .Where(h => h.MascotaId == id)
            .ExecuteDeleteAsync();

        await _context.Citas
            .Where(c => c.MascotaId == id)
            .ExecuteDeleteAsync();

        await _context.Consultas
            .Where(c => c.MascotaId == id)
            .ExecuteDeleteAsync();

        var affected = await _context.Mascotas
            .Where(m => m.Id == id)
            .ExecuteDeleteAsync();

        return affected > 0;
    }

    public async Task<bool> SaveChangesAsync()
        => await _context.SaveChangesAsync() > 0;
}
