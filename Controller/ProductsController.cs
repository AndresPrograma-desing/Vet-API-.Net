using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Services;
using DTOs;
using vet_api_Net.Constants;
using vet_api_Net.Routes;

namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ICreateProductService _createProductService;

    public ProductsController(IProductService productService, ICreateProductService createProductService)
    {
        _productService = productService;
        _createProductService = createProductService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        try
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost(Endpoints.ProductsController.Create)]
    public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDTO productDto)
    {
        try
        {
            var product = await _createProductService.CreateProductAsync(productDto);
            var result = new
            {
                codigo = product.Codigo,
                nombre = product.Nombre,
                descripcion = product.Descripcion,
                categoriaId = product.CategoriaId,
                tipo = product.Tipo,
                precio = product.Precio,
                precio_venta = product.PrecioVenta,
                stock = product.Stock,
                proveedor = product.Proveedor
            };
            return StatusCode(201, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete(Endpoints.ProductsController.Delete)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            var product = await _productService.DeleteProductAsync(id);
            return Ok(product);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPatch(Endpoints.ProductsController.Quantity)]
    public async Task<IActionResult> DecreaseStock(int id, [FromBody] DecreaseStockDTO dto)
    {
        try
        {
            if (dto == null || dto.Quantity <= 0)
                return BadRequest(new { error = ResponseMessagesProduct.ProductCantPositive });

            var product = await _productService.DecreaseStockAsync(id, dto.Quantity);
            var result = new
            {
                message = ResponseMessagesProduct.DecreaseStockSuccess,
                product = new
                {
                    id = product.Id,
                    codigo = product.Codigo,
                    nombre = product.Nombre,
                    stock = product.Stock
                }
            };
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseMessagesProduct.ErrorDecreasingStock });
        }
    }

    [HttpPut(Endpoints.ProductsController.UpdateProducto)]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductsUpdateDTO updatedProductDto)
    {
        try
        {
            var (product, hasChanges) = await _productService.UpdateProductAsync(id, updatedProductDto);
            var result = new
            {
                message = hasChanges 
                    ? ResponseMessagesProduct.UpdateProductSuccess 
                    : ResponseMessagesProduct.UpdateProductNoChanges,
                product
            };
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet(Endpoints.ProductsController.Categories)]
    public async Task<IActionResult> GetCategories()
    {
        try
        {
            var categories = await _productService.GetCategoriesAsync();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}