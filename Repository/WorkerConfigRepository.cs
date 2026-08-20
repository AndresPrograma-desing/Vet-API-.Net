using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Models;

namespace vet_api_Net.Repositories;

public class WorkerConfigRepository : IWorkerConfigRepository
{
    private readonly AppDbContext _context;

    public WorkerConfigRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<WorkerConfig?> GetByWorkerNameAsync(string workerName)
        => await _context.WorkerConfigs.FirstOrDefaultAsync(w => w.WorkerName == workerName);

    public async Task<List<WorkerConfig>> GetAllAsync()
        => await _context.WorkerConfigs.OrderBy(w => w.WorkerName).ToListAsync();

    public void AddWorkerConfig(WorkerConfig config)
        => _context.WorkerConfigs.Add(config);

    public void UpdateWorkerConfig(WorkerConfig config)
        => _context.WorkerConfigs.Update(config);

    public async Task<bool> SaveChangesAsync()
        => await _context.SaveChangesAsync() > 0;
}
