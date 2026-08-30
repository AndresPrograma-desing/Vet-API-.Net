using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Models;

//Describe: Repositorio de acceso a datos para el catálogo compartido de especies (mascotas y vacunas).
namespace vet_api_Net.Repositories;

public class EspecieRepository : IEspecieRepository
{
    private readonly AppDbContext _context;

    public EspecieRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Especie>> GetAllAsync()
        => await _context.Especies.AsNoTracking().OrderBy(e => e.Nombre).ToListAsync();

    public async Task<Especie?> GetByIdAsync(int id)
        => await _context.Especies.FindAsync(id);

    public async Task<Especie?> GetByNombreAsync(string nombre)
        => await _context.Especies.FirstOrDefaultAsync(e => e.Nombre.ToLower() == nombre.ToLower());

    public void AddEspecie(Especie especie)
        => _context.Especies.Add(especie);

    public async Task<bool> SaveChangesAsync()
        => await _context.SaveChangesAsync() > 0;
}
