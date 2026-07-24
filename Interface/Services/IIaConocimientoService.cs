using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Services;

public interface IIaConocimientoService
{
    Task<IaConocimiento?> GetByCategoriaAsync(string categoria);
    Task<IaConocimiento> SaveConfigAsync(string categoria, string reglasRespuesta, string baseConocimiento);
    Task<List<IaConocimiento>> GetAllConfigsAsync();
}
