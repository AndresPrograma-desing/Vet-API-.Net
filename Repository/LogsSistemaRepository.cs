using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Models;
using vet_api_Net.Interfaze.Repositories;

namespace vet_api_Net.Repositories;

public class LogsSistemaRepository : ILogsSistemaRepository
{
    private readonly AppDbContext _context;

    public LogsSistemaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddLogAsync(LogsSistema log)
    {
        await _context.LogsSistemas.AddAsync(log);
    }

    public async Task<List<LogsSistema>> GetLogsAsync(int pageNumber, int pageSize)
    {
        return await _context.LogsSistemas
            .Include(l => l.Usuario)
            .OrderByDescending(l => l.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
