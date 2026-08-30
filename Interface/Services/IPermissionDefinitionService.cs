using System.Threading.Tasks;
using DTOs;

namespace vet_api_Net.Interfaze.Services;

public interface IPermissionDefinitionService
{
    Task<PermissionCatalogResponseDTO> GetCatalogAsync();
}
