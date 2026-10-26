using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTOs;
using Microsoft.Extensions.Configuration;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Utilities;
using vet_api_Net.Models;

namespace vet_api_Net.Services;

public class ProductService : IProductService, ICreateProductService
{
    private readonly IProductRepository _repository;
    private readonly ICurrencyUtilities _currencyService;
    private readonly IMovimientosInventarioService _movimientosInventarioService;
    private readonly IHistorialPrecioService _historialPrecioService;

    public ProductService(
        IProductRepository repository,
        ICurrencyUtilities currencyService,
        IMovimientosInventarioService movimientosInventarioService,
        IHistorialPrecioService historialPrecioService)
    {
        _repository = repository;
        _currencyService = currencyService;
        _movimientosInventarioService = movimientosInventarioService;
        _historialPrecioService = historialPrecioService;
    }

    public async Task<List<ProductDTO>> GetProductsAsync(int? categoriaId, string searchTerm, decimal? maxPrice, int pageNumber, int pageSize)
    {
        var productsRaw = await _repository.GetActiveProductsAsync(ResponseMessagesProduct.NoProductName, categoriaId, searchTerm, maxPrice, pageNumber, pageSize);
        var money = await _currencyService.GetActiveMoneyTypeAsync();

        return productsRaw.Select(p => new ProductDTO
        {
            Id = p.Id,
            Codigo = p.Codigo,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            CategoriaId = p.CategoriaId,
            Tipo = p.Tipo,
            Precio = _currencyService.ConvertPrice(p.Precio, money),
            PrecioVenta = _currencyService.ConvertPrice(p.PrecioVenta, money),
            Stock = p.Stock,
            StockMinimo = p.StockMinimo,
            Proveedor = p.Proveedor
        }).ToList();
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

    public async Task<Producto> DecreaseStockAsync(int id, int quantity, int? userId = null, string? reason = null, int? referenceId = null)
    {
        if (quantity <= 0)
            throw new ArgumentException(ResponseMessagesProduct.ProductCantPositive);

        var product = await _repository.GetProductByIdAsync(id);
        if (product == null)
            throw new KeyNotFoundException(ResponseMessagesProduct.ProductNotFound);

        var currentStock = product.Stock ?? 0;
        if (currentStock < quantity)
            throw new InvalidOperationException(ResponseMessagesProduct.NotSufficientStock);

        var newStock = currentStock - quantity;
        product.Stock = newStock;
        _repository.UpdateProduct(product);
        await _repository.SaveChangesAsync();

        if (userId.HasValue)
        {
            await _movimientosInventarioService.RegisterMovementAsync(
                product.Id,
                InventoryVariables.TipoSalida,
                quantity,
                currentStock,
                newStock,
                reason ?? ResponseMessagesProduct.DefaultDecreaseMotivo,
                userId.Value,
                referenceId);
        }

        return product;
    }
    public async Task<(Producto Product, bool HasChanges)> UpdateProductAsync(int id, ProductsUpdateDTO updatedProductDto, int? userId = null)
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

        var previousPrice = existingProduct.Precio;
        var previousStock = existingProduct.Stock ?? 0;

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

        if (userId.HasValue)
        {
            if (previousPrice != targetPrecio)
            {
                await _historialPrecioService.RegisterPriceChangeAsync(
                    existingProduct.Id,
                    previousPrice,
                    targetPrecio,
                    userId.Value,
                    ResponseMessagesProduct.DefaultPriceChangeMotivo);
            }

            var newStock = targetStock ?? 0;
            if (previousStock != newStock)
            {
                await _movimientosInventarioService.RegisterMovementAsync(
                    existingProduct.Id,
                    newStock > previousStock ? InventoryVariables.TipoEntrada : InventoryVariables.TipoSalida,
                    Math.Abs(newStock - previousStock),
                    previousStock,
                    newStock,
                    ResponseMessagesProduct.DefaultUpdateStockMotivo,
                    userId.Value);
            }
        }

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