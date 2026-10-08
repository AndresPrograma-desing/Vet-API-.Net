using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;

//Describe: Contrato de acceso a datos para consultar, actualizar y eliminar mascotas con sus relaciones.
namespace vet_api_Net.Interfaze.Repositories;

public interface IPetsRepository
{
    Task<(List<Mascota> Items, int TotalCount)> GetPagedWithRelationsAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null);
    Task<Mascota?> GetByIdWithRelationsAsync(int id);
    Task<List<Mascota>> GetByClientNameWithRelationsAsync(string lowerClientName);
    Task<bool> ExistsAsync(int id);
    void Update(Mascota mascota);
    Task<bool> DeleteWithDependenciesAsync(int id);
    Task<bool> SaveChangesAsync();
}
