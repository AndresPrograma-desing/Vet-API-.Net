using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaces.Repositories;

public interface IProductRepository
{
    Task<MoneyType?> GetMoneyTypeByIdAsync(int id);
    Task<List<Producto>> GetActiveProductsAsync(string noProductName);
    Task<Producto?> GetProductByIdAsync(int id);
    Task<List<CategoriasProducto>> GetCategoriesAsync();
    void AddProduct(Producto product);
    void UpdateProduct(Producto product);
    void DeleteProduct(Producto product);
    Task<bool> SaveChangesAsync();
    Task<bool> ExistsByCodeOrNameAsync(string code, string name);
    Task<bool> CategoryExistsAsync(int categoryId);
}