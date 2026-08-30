using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Repositories;

public interface IPermissionDefinitionRepository
{
    Task<List<PermissionModule>> GetAllModulesOrderedAsync();
}
