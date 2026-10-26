using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Extensions;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Models;

namespace vet_api_Net.Repositories;

public class ServicioRepository : IServicioRepository
{
    private readonly AppDbContext _context;

    public ServicioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(List<Servicio> Items, int TotalCount)> GetPagedAsync(int pageNumber = 1, int pageSize = 10, bool? active = null, string? searchTerm = null)
    {
        var query = _context.Servicios.AsQueryable();

        if (active.HasValue)
            query = query.Where(s => s.Activo == active.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var cleanSearch = searchTerm.Trim().ToLower();
            query = query.Where(s => s.Nombre.ToLower().Contains(cleanSearch));
        }

        return await query
            .OrderBy(s => s.Nombre)
            .ToPagedResultAsync(pageNumber, pageSize);
    }

    public async Task<Servicio?> GetByIdAsync(int id)
        => await _context.Servicios.FindAsync(id);

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        => await _context.Servicios.AnyAsync(s => s.Nombre.ToLower() == name.ToLower() && (!excludeId.HasValue || s.Id != excludeId.Value));

    public void AddServicio(Servicio servicio)
        => _context.Servicios.Add(servicio);

    public void UpdateServicio(Servicio servicio)
        => _context.Servicios.Update(servicio);

    public void DeleteServicio(Servicio servicio)
        => _context.Servicios.Remove(servicio);

    public async Task<bool> SaveChangesAsync()
        => await _context.SaveChangesAsync() > 0;
}
