using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Repositories;

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
}