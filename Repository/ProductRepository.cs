using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Extensions;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Models;

namespace vet_api_Net.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MoneyType?> GetMoneyTypeByIdAsync(int id)
    {
        return await _context.MoneyTypes.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
    }
    public async Task<List<Producto>> GetActiveProductsAsync(string noProductName, int? categoriaId, string searchTerm, decimal? maxPrice, int pageNumber, int pageSize)
{
    var query = _context.Productos
        .AsNoTracking()
        .Where(p => p.Nombre != noProductName);

    if (categoriaId.HasValue)
    {
        query = query.Where(p => p.CategoriaId == categoriaId.Value);
    }

    if (!string.IsNullOrWhiteSpace(noProductName))
    {
        var cleanNoProduct = noProductName.Trim().ToLower();
        query = query.Where(p => p.Nombre.ToLower() != cleanNoProduct);
    }

    if (!string.IsNullOrWhiteSpace(searchTerm))
    {
        var cleanSearch = searchTerm.Trim().ToLower();
        query = query.Where(p => p.Nombre.ToLower().Contains(cleanSearch));
    }

    return await query.ToPagedListAsync(pageNumber, pageSize);
}
    public async Task<Producto?> GetProductByIdAsync(int id)
    {
        return await _context.Productos.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<CategoriasProducto>> GetCategoriesAsync()
    {
        return await _context.CategoriasProductos.AsNoTracking().ToListAsync();
    }

    public void AddProduct(Producto product)
    {
        _context.Productos.Add(product);
    }

    public void UpdateProduct(Producto product)
    {
        _context.Productos.Update(product);
    }

    public void DeleteProduct(Producto product)
    {
        _context.Productos.Remove(product);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}