using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Extensions;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Models;

//Describe: Repositorio de acceso a datos para el catálogo de vacunas, sus etapas de esquema y sus lotes.
namespace vet_api_Net.Repositories;

public class VaccineRepository : IVaccineRepository
{
    private readonly AppDbContext _context;

    public VaccineRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(List<Vaccine> Items, int TotalCount)> GetPagedAsync(int pageNumber = 1, int pageSize = 10, string? species = null, bool? active = null, string? searchTerm = null)
    {
        var query = _context.Vaccines.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(species))
            query = query.Where(v => v.Especie.Nombre.ToLower() == species.Trim().ToLower());

        if (active.HasValue)
            query = query.Where(v => v.Active == active.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var cleanSearch = searchTerm.Trim().ToLower();
            query = query.Where(v => v.Name.ToLower().Contains(cleanSearch));
        }

        return await query
            .Include(v => v.SchemeStages)
            .Include(v => v.Especie)
            .OrderBy(v => v.Name)
            .ToPagedResultAsync(pageNumber, pageSize);
    }

    public async Task<Vaccine?> GetByIdAsync(int id)
        => await _context.Vaccines.FindAsync(id);

    public async Task<Vaccine?> GetByIdWithStagesAsync(int id)
        => await _context.Vaccines.Include(v => v.SchemeStages).Include(v => v.Especie).FirstOrDefaultAsync(v => v.Id == id);

    public void AddVaccine(Vaccine vaccine)
        => _context.Vaccines.Add(vaccine);

    public void UpdateVaccine(Vaccine vaccine)
        => _context.Vaccines.Update(vaccine);

    public void DeleteVaccine(Vaccine vaccine)
        => _context.Vaccines.Remove(vaccine);

    public async Task<VaccineSchemeStage?> GetSchemeStageByIdAsync(int id)
        => await _context.VaccineSchemeStages.FindAsync(id);

    public void AddSchemeStage(VaccineSchemeStage stage)
        => _context.VaccineSchemeStages.Add(stage);

    public void RemoveSchemeStage(VaccineSchemeStage stage)
        => _context.VaccineSchemeStages.Remove(stage);

    public async Task<(List<VaccineBatch> Items, int TotalCount)> GetBatchesPagedAsync(int vaccineId, int pageNumber, int pageSize, bool? active = null, string? searchTerm = null)
    {
        var query = _context.VaccineBatches.AsNoTracking().Where(b => b.VaccineId == vaccineId);

        if (active.HasValue)
            query = query.Where(b => b.Active == active.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var cleanSearch = searchTerm.Trim().ToLower();
            query = query.Where(b => b.BatchNumber.ToLower().Contains(cleanSearch) || b.Laboratory.ToLower().Contains(cleanSearch));
        }

        return await query
            .OrderBy(b => b.ExpirationDate)
            .ToPagedResultAsync(pageNumber, pageSize);
    }

    public async Task<VaccineBatch?> GetBatchByIdAsync(int id)
        => await _context.VaccineBatches.FindAsync(id);

    public void AddBatch(VaccineBatch batch)
        => _context.VaccineBatches.Add(batch);

    public void UpdateBatch(VaccineBatch batch)
        => _context.VaccineBatches.Update(batch);

    public async Task<bool> SaveChangesAsync()
        => await _context.SaveChangesAsync() > 0;
}
