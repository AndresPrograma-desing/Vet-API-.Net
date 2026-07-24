using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Models;
using vet_api_Net.Interfaze.Repositories;

namespace vet_api_Net.Repositories;

//Describe: Repositorio para la gestión de acceso a datos de reglas y base de conocimiento de la IA.
public class IaConocimientoRepository : IIaConocimientoRepository
{
    private readonly AppDbContext _context;

    public IaConocimientoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IaConocimiento?> GetByIdAsync(int id)
        => await _context.IaConocimientos.FindAsync(id);

    public async Task<IaConocimiento?> GetByCategoriaAsync(string categoria)
        => await _context.IaConocimientos
            .FirstOrDefaultAsync(x => x.Categoria.ToLower() == categoria.ToLower());

    public async Task<List<IaConocimiento>> GetAllAsync()
        => await _context.IaConocimientos.ToListAsync();

    public async Task<IaConocimiento> AddAsync(IaConocimiento iaConocimiento)
    {
        await _context.IaConocimientos.AddAsync(iaConocimiento);
        return iaConocimiento;
    }

    public async Task<IaConocimiento> UpdateAsync(IaConocimiento iaConocimiento)
    {
        _context.IaConocimientos.Update(iaConocimiento);
        return await Task.FromResult(iaConocimiento);
    }

    public async Task<bool> SaveChangesAsync()
        => await _context.SaveChangesAsync() > 0;
}
