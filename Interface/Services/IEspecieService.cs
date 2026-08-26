using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Services;

public interface IEspecieService
{
    Task<List<EspecieDTO>> GetAllAsync();
    Task<EspecieDTO> CreateAsync(EspecieCreateDTO dto);
    Task<Especie> GetOrCreateByNombreAsync(string nombre);
}
