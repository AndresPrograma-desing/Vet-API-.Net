using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Extensions;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Models;

namespace vet_api_Net.Repositories;

public class WorkerProcessEventRepository(AppDbContext context) : IWorkerProcessEventRepository
{
    private readonly AppDbContext _context = context;

    public async Task<WorkerProcessEvent?> GetActiveByNameAndVinculationAsync(string nameEvent, string? vinculationData)
    {
        return await _context.WorkerProcessEvents
            .Where(e => e.NameEvent == nameEvent && !e.IsResolved && e.VinculationData == vinculationData)
            .OrderByDescending(e => e.Created)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(WorkerProcessEvent processEvent)
    {
        await _context.WorkerProcessEvents.AddAsync(processEvent);
    }

    public async Task<(List<WorkerProcessEvent> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? typeEvent,
        bool? isResolved,
        DateTime? fromDate,
        DateTime? toDate)
    {
        var query = _context.WorkerProcessEvents.AsQueryable();

        if (!string.IsNullOrWhiteSpace(typeEvent))
            query = query.Where(e => e.TypeEvent == typeEvent);

        if (isResolved.HasValue)
            query = query.Where(e => e.IsResolved == isResolved.Value);

        if (fromDate.HasValue)
            query = query.Where(e => e.Created >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(e => e.Created <= toDate.Value);

        query = query.OrderByDescending(e => e.Created);

        return await query.ToPagedResultAsync(pageNumber, pageSize);
    }

    public async Task<List<WorkerProcessEvent>> GetResolvedOlderThanAsync(DateTime threshold)
    {
        return await _context.WorkerProcessEvents
            .Where(e => e.IsResolved && e.ResolvedAt.HasValue && e.ResolvedAt.Value < threshold)
            .ToListAsync();
    }

    public void RemoveRange(IEnumerable<WorkerProcessEvent> processEvents)
    {
        _context.WorkerProcessEvents.RemoveRange(processEvents);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
