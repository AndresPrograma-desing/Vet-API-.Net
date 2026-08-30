using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Models;
using vet_api_Net.Interfaze.Repositories;

namespace vet_api_Net.Repositories;

public class PermissionDefinitionRepository : IPermissionDefinitionRepository
{
    private readonly AppDbContext _context;

    public PermissionDefinitionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PermissionModule>> GetAllModulesOrderedAsync()
        => await _context.PermissionModules
            .Include(m => m.Permissions.OrderBy(p => p.SortOrder))
            .OrderBy(m => m.SortOrder)
            .ToListAsync();
}
