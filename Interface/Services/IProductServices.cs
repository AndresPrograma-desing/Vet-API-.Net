using vet_api_Net.Data;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Models;
using DTOs;

namespace vet_api_Net.Services;

public interface IProductService
{
    Task<List<ProductDTO>> GetProductsAsync(int? categoriaId, string searchTerm, decimal? maxPrice, int pageNumber, int pageSize);

    Task<Producto> DeleteProductAsync(int id);

    Task<Producto> DecreaseStockAsync(int id, int quantity);

    Task<(Producto Product, bool HasChanges)> UpdateProductAsync(int id, ProductsUpdateDTO updatedProductDto);
    Task<List<CategoryProductsDTO>> GetCategoriesAsync();

}
