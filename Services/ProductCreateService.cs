using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;
using DTOs;
using vet_api_Net.Constants;
using vet_api_Net.Interfaces.Services;
using vet_api_Net.Interfaces.Repositories;

namespace vet_api_Net.Services
{
    public class ProductCreateService : ICreateProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductCreateService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Producto> CreateProductAsync(ProductCreateDTO productDto)
        {
            bool exists = await _productRepository.ExistsByCodeOrNameAsync(productDto.Codigo, productDto.Nombre);

            if (exists)
            {
                throw new InvalidOperationException(ResponseMessagesProduct.ResponseMessagesProductCreate.ProducNameOrCodeExists);
            }

            if (productDto.CategoriaId.HasValue)
            {
                var categoryExists = await _productRepository.CategoryExistsAsync(productDto.CategoriaId.Value);
                if (!categoryExists)
                    throw new KeyNotFoundException(ResponseMessagesProduct.ResponseMessagesProductCreate.CategoryNotFound);
            }

            ArgumentException.ThrowIfNullOrEmpty(productDto.Nombre, ResponseMessagesProduct.ResponseMessagesProductCreate.NameAndCodeRequired);
            ArgumentException.ThrowIfNullOrEmpty(productDto.Codigo, ResponseMessagesProduct.ResponseMessagesProductCreate.NameAndCodeRequired);

            if (productDto.Precio < 0 || productDto.PrecioVenta < 0)
            {
                throw new ArgumentException(ResponseMessagesProduct.ResponseMessagesProductCreate.PriceCannotBeNegative);
            }

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

            _productRepository.AddProduct(product);
            await _productRepository.SaveChangesAsync();

            return product;
        }
    }
}