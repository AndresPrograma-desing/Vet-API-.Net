using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Models;
using vet_api_Net.Interfaze.Repositories;
using DTOs;

namespace vet_api_Net.Repositories;

public class SystemConfigRespository: ISystemConfigRepository
{
    private readonly AppDbContext _context;

    public SystemConfigRespository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SystemConfig?> GetSystemConfigAsync()
    {
        return  await _context.SystemConfigs.FirstOrDefaultAsync();
    }
}