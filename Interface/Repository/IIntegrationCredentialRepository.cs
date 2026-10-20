using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;

//Describe: Contrato de acceso a datos para las credenciales de integraciones externas (ej. API keys de Gemini) rotables por proveedor.
namespace vet_api_Net.Interfaze.Repositories;

public interface IIntegrationCredentialRepository
{
    Task<List<IntegrationCredential>> GetActiveByProviderAsync(string provider);
    Task<List<IntegrationCredential>> GetAllAsync();
    Task<IntegrationCredential?> GetByIdAsync(int id);
    void Add(IntegrationCredential credential);
    void Update(IntegrationCredential credential);
    void Delete(IntegrationCredential credential);
    Task<bool> SaveChangesAsync();
}
