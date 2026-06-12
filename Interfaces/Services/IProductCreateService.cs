using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaces.Services;

public interface ICreateProductService
{
    Task<Producto> CreateProductAsync(ProductCreateDTO productDto);
}