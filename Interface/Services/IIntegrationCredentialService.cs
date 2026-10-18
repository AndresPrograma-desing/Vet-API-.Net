using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;

//Describe: Contrato del servicio de administración de credenciales de integraciones externas por proveedor.
namespace vet_api_Net.Interfaze.Services;

public interface IIntegrationCredentialService
{
    Task<List<IntegrationCredentialDTO>> GetByProviderAsync(string provider);
    Task<IntegrationCredentialDTO> CreateAsync(CreateIntegrationCredentialDTO dto);
    Task<IntegrationCredentialDTO> UpdateAsync(int id, UpdateIntegrationCredentialDTO dto);
    Task DeleteAsync(int id);
}
