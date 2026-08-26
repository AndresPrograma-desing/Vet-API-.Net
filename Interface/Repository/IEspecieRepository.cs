using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Repositories;

public interface IEspecieRepository
{
    Task<List<Especie>> GetAllAsync();
    Task<Especie?> GetByIdAsync(int id);
    Task<Especie?> GetByNombreAsync(string nombre);
    void AddEspecie(Especie especie);
    Task<bool> SaveChangesAsync();
}
