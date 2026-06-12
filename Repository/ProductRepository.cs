using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Models;
using vet_api_Net.Interfaces.Repositories;

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

    public async Task<List<Producto>> GetActiveProductsAsync(string noProductName)
    {
        return await _context.Productos
            .Where(p => p.Nombre != noProductName)
            .ToListAsync();
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

    public async Task<bool> ExistsByCodeOrNameAsync(string code, string name)
    {
        return await _context.Productos.AnyAsync(p => p.Codigo == code || p.Nombre == name);
    }

    public async Task<bool> CategoryExistsAsync(int categoryId)
    {
        return await _context.CategoriasProductos.AnyAsync(c => c.Id == categoryId);
    }
}