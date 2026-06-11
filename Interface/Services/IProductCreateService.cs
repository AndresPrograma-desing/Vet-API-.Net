using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Services;

public interface ICreateProductService
{
    Task<Producto> CreateProductAsync(ProductCreateDTO productDto);
}