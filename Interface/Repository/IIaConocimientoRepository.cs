using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Repositories;

public interface IIaConocimientoRepository
{
    Task<IaConocimiento?> GetByIdAsync(int id);
    Task<IaConocimiento?> GetByCategoriaAsync(string categoria);
    Task<List<IaConocimiento>> GetAllAsync();
    Task<IaConocimiento> AddAsync(IaConocimiento iaConocimiento);
    Task<IaConocimiento> UpdateAsync(IaConocimiento iaConocimiento);
    Task<bool> SaveChangesAsync();
}
