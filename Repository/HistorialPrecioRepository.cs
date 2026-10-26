using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Extensions;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Models;

namespace vet_api_Net.Repositories;

public class HistorialPrecioRepository : IHistorialPrecioRepository
{
    private readonly AppDbContext _context;

    public HistorialPrecioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(List<HistorialPrecio> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        int? productId = null)
    {
        var query = _context.HistorialPrecios.AsQueryable();

        if (productId.HasValue)
            query = query.Where(h => h.ProductoId == productId.Value);

        return await query
            .Include(h => h.Producto)
            .Include(h => h.Usuario)
            .OrderByDescending(h => h.FechaCambio)
            .ToPagedResultAsync(pageNumber, pageSize);
    }

    public void AddHistory(HistorialPrecio history)
        => _context.HistorialPrecios.Add(history);

    public async Task<bool> SaveChangesAsync()
        => await _context.SaveChangesAsync() > 0;
}
