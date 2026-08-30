using System.Linq;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;

namespace vet_api_Net.Services;

public class PermissionDefinitionService : IPermissionDefinitionService
{
    private readonly IPermissionDefinitionRepository _repository;

    public PermissionDefinitionService(IPermissionDefinitionRepository repository)
    {
        _repository = repository;
    }

    public async Task<PermissionCatalogResponseDTO> GetCatalogAsync()
    {
        var modules = await _repository.GetAllModulesOrderedAsync();

        return new PermissionCatalogResponseDTO
        {
            Modules = modules.Select(m => new PermissionModuleDTO
            {
                Module = m.ModuleKey,
                Label = m.Label,
                Icon = m.Icon,
                Permissions = m.Permissions.Select(p => new PermissionItemDTO
                {
                    Key = p.Key,
                    Label = p.Label
                }).ToList()
            }).ToList(),
            Roles = RoleLabels.Values.Select(r => new RoleCatalogItemDTO
            {
                Key = r.Key,
                Label = r.Value
            }).ToList()
        };
    }
}
