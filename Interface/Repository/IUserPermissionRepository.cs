using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Repositories;

public interface IUserPermissionRepository
{
    Task<UserPermission?> GetByUserIdAsync(int userId);
    Task<UserPermission> UpsertAsync(int userId, string permissionsJson);
}
