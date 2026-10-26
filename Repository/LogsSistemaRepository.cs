using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Extensions;
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

    public async Task<(List<LogsSistema> Items, int TotalCount)> GetLogsAsync(
        int pageNumber,
        int pageSize,
        int? userId = null,
        string? action = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null)
    {
        var query = _context.LogsSistemas.AsQueryable();

        if (userId.HasValue)
            query = query.Where(l => l.UsuarioId == userId.Value);

        if (!string.IsNullOrWhiteSpace(action))
        {
            var cleanAction = action.Trim().ToLower();
            query = query.Where(l => l.Accion.ToLower().Contains(cleanAction));
        }

        if (dateFrom.HasValue)
            query = query.Where(l => l.CreatedAt >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(l => l.CreatedAt <= dateTo.Value);

        return await query
            .Include(l => l.Usuario)
            .OrderByDescending(l => l.CreatedAt)
            .ToPagedResultAsync(pageNumber, pageSize);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
