using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Models;
using vet_api_Net.Interfaze.Repositories;

namespace vet_api_Net.Repositories;

public class UserPermissionRepository : IUserPermissionRepository
{
    private readonly AppDbContext _context;

    public UserPermissionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserPermission?> GetByUserIdAsync(int userId)
    {
        return await _context.UserPermissions
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<UserPermission> UpsertAsync(int userId, string permissionsJson)
    {
        var entity = await _context.UserPermissions.FirstOrDefaultAsync(p => p.UserId == userId);
        var now = DateTime.Now;

        if (entity == null)
        {
            entity = new UserPermission
            {
                UserId = userId,
                Permissions = permissionsJson,
                CreatedAt = now,
                UpdatedAt = now
            };
            _context.UserPermissions.Add(entity);
        }
        else
        {
            entity.Permissions = permissionsJson;
            entity.UpdatedAt = now;
        }

        await _context.SaveChangesAsync();
        return entity;
    }
}
