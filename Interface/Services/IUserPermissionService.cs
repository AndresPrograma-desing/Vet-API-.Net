using System.Collections.Generic;
using System.Threading.Tasks;

namespace vet_api_Net.Interfaze.Services;

public interface IUserPermissionService
{
    Task<List<string>?> GetPermissionsAsync(int userId);
    Task<List<string>> SetPermissionsAsync(int userId, List<string> permissions);
}
