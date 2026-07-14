using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using DTOs;
using vet_api_Net.Models;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Utilities;
using vet_api_Net.Utilities;

namespace vet_api_Net.Services;

public class ProductService : IProductService, ICreateProductService
{
    private readonly IProductRepository _repository;
    private readonly ICurrencyService _currencyService;

    public ProductService(IProductRepository repository, ICurrencyService currencyService)
    {
        _repository = repository;
        _currencyService = currencyService;
    }

    public async Task<List<ProductDTO>> GetAllProductsAsync()
    {
        var productsRaw = await _repository.GetActiveProductsAsync(ResponseMessagesProduct.NoProductName);
        var productList = new List<ProductDTO>();

        foreach (var p in productsRaw)
        {
            productList.Add(new ProductDTO
            {
                Id = p.Id,
                Codigo = p.Codigo,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                CategoriaId = p.CategoriaId,
                Tipo = p.Tipo,
                Precio = await _currencyService.ConvertPriceAsync(p.Precio),
                PrecioVenta = await _currencyService.ConvertPriceAsync(p.PrecioVenta),
                Stock = p.Stock,
                StockMinimo = p.StockMinimo,
                Proveedor = p.Proveedor
            });
        }

        return productList;
    }

    public async Task<Producto> CreateProductAsync(ProductCreateDTO productDto)
    {
        ArgumentNullException.ThrowIfNull(productDto);

        if (productDto.Precio < 0 || productDto.PrecioVenta < 0)
            throw new ArgumentException(ResponseMessagesProduct.ProductCantNegative);

        var product = new Producto
        {
            Codigo = productDto.Codigo,
            Nombre = productDto.Nombre,
            Descripcion = productDto.Descripcion,
            CategoriaId = productDto.CategoriaId,
            Tipo = productDto.Tipo,
            Precio = productDto.Precio,
            PrecioVenta = productDto.PrecioVenta,
            Stock = productDto.Stock,
            StockMinimo = productDto.StockMinimo,
            UnidadMedida = productDto.UnidadMedida,
            Proveedor = productDto.Proveedor
        };

        _repository.AddProduct(product);
        await _repository.SaveChangesAsync();

        return product;
    }

    public async Task<Producto> DeleteProductAsync(int id)
    {
        var product = await _repository.GetProductByIdAsync(id);
        if (product == null)
            throw new KeyNotFoundException(ResponseMessagesProduct.ProductNotFound);

        _repository.DeleteProduct(product);
        await _repository.SaveChangesAsync();

        return product;
    }

    public async Task<Producto> DecreaseStockAsync(int id, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(ResponseMessagesProduct.ProductCantPositive);

        var product = await _repository.GetProductByIdAsync(id);
        if (product == null)
            throw new KeyNotFoundException(ResponseMessagesProduct.ProductNotFound);

        var currentStock = product.Stock ?? 0;
        if (currentStock < quantity)
            throw new InvalidOperationException(ResponseMessagesProduct.NotSufficientStock);

        product.Stock = currentStock - quantity;
        _repository.UpdateProduct(product);
        await _repository.SaveChangesAsync();

        return product;
    }
public async Task<(Producto Product, bool HasChanges)> UpdateProductAsync(int id, ProductsUpdateDTO updatedProductDto)
{
    ArgumentNullException.ThrowIfNull(updatedProductDto);

    if ((updatedProductDto.Precio.HasValue && updatedProductDto.Precio < 0) || (updatedProductDto.PrecioVenta.HasValue && updatedProductDto.PrecioVenta < 0))
        throw new ArgumentException(ResponseMessagesProduct.ProductCantNegative);

    var existingProduct = await _repository.GetProductByIdAsync(id);
    if (existingProduct == null)
        throw new KeyNotFoundException(ResponseMessagesProduct.ProductNotFound);

    decimal targetPrecio = existingProduct.Precio;
    if (updatedProductDto.Precio.HasValue)
    {
        targetPrecio = await _currencyService.ConvertToUsdAsync(updatedProductDto.Precio.Value);
    }

    decimal targetPrecioVenta = existingProduct.PrecioVenta;
    if (updatedProductDto.PrecioVenta.HasValue)
    {
        targetPrecioVenta = await _currencyService.ConvertToUsdAsync(updatedProductDto.PrecioVenta.Value);
    }

    string targetCodigo = updatedProductDto.Codigo ?? existingProduct.Codigo;
    string targetNombre = updatedProductDto.Nombre ?? existingProduct.Nombre;
    string? targetDescripcion = updatedProductDto.Descripcion ?? existingProduct.Descripcion;
    int? targetCategoriaId = updatedProductDto.CategoriaId ?? existingProduct.CategoriaId;
    string targetTipo = updatedProductDto.Tipo ?? existingProduct.Tipo;
    int? targetStock = updatedProductDto.Stock ?? existingProduct.Stock;
    int? targetStockMinimo = updatedProductDto.StockMinimo ?? existingProduct.StockMinimo;
    string? targetProveedor = updatedProductDto.Proveedor ?? existingProduct.Proveedor;
    string? targetUnidadMedida = updatedProductDto.UnidadMedida ?? existingProduct.UnidadMedida;

    bool hasChanges = 
        existingProduct.Precio != targetPrecio ||
        existingProduct.PrecioVenta != targetPrecioVenta ||
        existingProduct.Codigo != targetCodigo ||
        existingProduct.Nombre != targetNombre ||
        existingProduct.Descripcion != targetDescripcion ||
        existingProduct.CategoriaId != targetCategoriaId ||
        existingProduct.Tipo != targetTipo ||
        existingProduct.Stock != targetStock ||
        existingProduct.StockMinimo != targetStockMinimo ||
        existingProduct.Proveedor != targetProveedor ||
        existingProduct.UnidadMedida != targetUnidadMedida;

    if (!hasChanges)
    {
        return (existingProduct, false);
    }

    existingProduct.Precio = targetPrecio;
    existingProduct.PrecioVenta = targetPrecioVenta;
    existingProduct.Codigo = targetCodigo;
    existingProduct.Nombre = targetNombre;
    existingProduct.Descripcion = targetDescripcion;
    existingProduct.CategoriaId = targetCategoriaId;
    existingProduct.Tipo = targetTipo;
    existingProduct.Stock = targetStock;
    existingProduct.StockMinimo = targetStockMinimo;
    existingProduct.Proveedor = targetProveedor;
    existingProduct.UnidadMedida = targetUnidadMedida;

    _repository.UpdateProduct(existingProduct);
    await _repository.SaveChangesAsync();

    return (existingProduct, true);
}

    public async Task<List<CategoryProductsDTO>> GetCategoriesAsync()
    {
        var categories = await _repository.GetCategoriesAsync();
        return categories.Select(category => new CategoryProductsDTO
        {
            Id = category.Id,
            Nombre = category.Nombre,
            Descripcion = category.Descripcion
        }).ToList();
    }
}