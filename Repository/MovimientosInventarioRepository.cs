using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Extensions;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Models;

namespace vet_api_Net.Repositories;

public class MovimientosInventarioRepository : IMovimientosInventarioRepository
{
    private readonly AppDbContext _context;

    public MovimientosInventarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(List<MovimientosInventario> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        int? productId = null,
        string? movementType = null)
    {
        var query = _context.MovimientosInventarios.AsQueryable();

        if (productId.HasValue)
            query = query.Where(m => m.ProductoId == productId.Value);

        if (!string.IsNullOrWhiteSpace(movementType))
            query = query.Where(m => m.TipoMovimiento.ToLower() == movementType.Trim().ToLower());

        return await query
            .Include(m => m.Producto)
            .Include(m => m.Usuario)
            .OrderByDescending(m => m.FechaMovimiento)
            .ToPagedResultAsync(pageNumber, pageSize);
    }

    public async Task<List<Producto>> GetProductsWithLowStockAsync()
    {
        return await _context.Productos
            .Where(p => p.StockMinimo.HasValue && p.Stock.HasValue && p.Stock <= p.StockMinimo)
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }

    public void AddMovement(MovimientosInventario movement)
        => _context.MovimientosInventarios.Add(movement);

    public async Task<bool> SaveChangesAsync()
        => await _context.SaveChangesAsync() > 0;
}
