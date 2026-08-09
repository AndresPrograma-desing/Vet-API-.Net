using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;

namespace vet_api_Net.Services;

public class UserPermissionService : IUserPermissionService
{
    private readonly IUserPermissionRepository _repository;
    private readonly IUsersRepository _usersRepository;

    public UserPermissionService(IUserPermissionRepository repository, IUsersRepository usersRepository)
    {
        _repository = repository;
        _usersRepository = usersRepository;
    }

    public async Task<List<string>?> GetPermissionsAsync(int userId)
    {
        var entity = await _repository.GetByUserIdAsync(userId);
        if (entity == null) return null;

        return JsonSerializer.Deserialize<List<string>>(entity.Permissions) ?? new List<string>();
    }

    public async Task<List<string>> SetPermissionsAsync(int userId, List<string> permissions)
    {
        var user = await _usersRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException(ResponseMessagesUserPermissions.UserNotFound);
        }

        var permissionsJson = JsonSerializer.Serialize(permissions);
        var entity = await _repository.UpsertAsync(userId, permissionsJson);

        return JsonSerializer.Deserialize<List<string>>(entity.Permissions) ?? new List<string>();
    }
}
