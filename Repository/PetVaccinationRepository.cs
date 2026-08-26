using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Extensions;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Models;

//Describe: Repositorio de acceso a datos para el flujo de aplicación de vacunas a mascotas (carnet, semaforización, recordatorios y facturación).
namespace vet_api_Net.Repositories;

public class PetVaccinationRepository : IPetVaccinationRepository
{
    private readonly AppDbContext _context;

    public PetVaccinationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Mascota?> GetMascotaByIdAsync(int id)
        => await _context.Mascotas.Include(m => m.Cliente).Include(m => m.Especie).FirstOrDefaultAsync(m => m.Id == id);

    public async Task<Usuario?> GetDoctorByIdAsync(int id)
        => await _context.Usuarios.FindAsync(id);

    public async Task<Vaccine?> GetVaccineByIdAsync(int id)
        => await _context.Vaccines.FindAsync(id);

    public async Task<VaccineBatch?> GetBatchByIdAsync(int id)
        => await _context.VaccineBatches.FindAsync(id);

    public async Task<List<VaccineSchemeStage>> GetSchemeStagesByVaccineIdAsync(int vaccineId)
        => await _context.VaccineSchemeStages
            .Where(s => s.VaccineId == vaccineId)
            .OrderBy(s => s.DoseNumber)
            .ToListAsync();

    public async Task<List<Vaccine>> GetActiveVaccinesByEspecieIdAsync(int especieId)
        => await _context.Vaccines
            .Where(v => v.Active && v.EspecieId == especieId)
            .ToListAsync();

    public async Task<int> CountAppliedDosesAsync(int mascotaId, int vaccineId)
        => await _context.PetVaccinations
            .CountAsync(v => v.MascotaId == mascotaId && v.VaccineId == vaccineId && v.Status == "Applied");

    public async Task<PetVaccination?> GetLastApplicationAsync(int mascotaId, int vaccineId)
        => await _context.PetVaccinations
            .Where(v => v.MascotaId == mascotaId && v.VaccineId == vaccineId && v.Status == "Applied")
            .OrderByDescending(v => v.ApplicationDate)
            .FirstOrDefaultAsync();

    public async Task<PetVaccination?> GetByIdAsync(int id)
        => await _context.PetVaccinations.FindAsync(id);

    public async Task<PetVaccination?> GetByIdWithDetailsAsync(int id)
        => await _context.PetVaccinations
            .Include(v => v.Vaccine)
            .Include(v => v.VaccineBatch)
            .Include(v => v.Doctor)
            .FirstOrDefaultAsync(v => v.Id == id);

    public void AddPetVaccination(PetVaccination petVaccination)
        => _context.PetVaccinations.Add(petVaccination);

    public void UpdatePetVaccination(PetVaccination petVaccination)
        => _context.PetVaccinations.Update(petVaccination);

    public void UpdateBatch(VaccineBatch batch)
        => _context.VaccineBatches.Update(batch);

    public async Task<List<PetVaccination>> GetHistoryByMascotaIdAsync(int mascotaId, string? status = null)
    {
        var query = _context.PetVaccinations
            .Where(v => v.MascotaId == mascotaId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(v => v.Status == status);

        return await query
            .Include(v => v.Vaccine)
            .Include(v => v.VaccineBatch)
            .Include(v => v.Doctor)
            .OrderByDescending(v => v.ApplicationDate)
            .ToListAsync();
    }

    public async Task<(List<PetVaccination> Items, int TotalCount)> GetPendingThisWeekAsync(int pageNumber, int pageSize, string? searchTerm = null)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var weekAhead = today.AddDays(7);

        var query = _context.PetVaccinations
            .Where(v => v.Status == "Applied" && v.NextDoseDate != null && v.NextDoseDate >= today && v.NextDoseDate <= weekAhead)
            .Include(v => v.Vaccine)
            .Include(v => v.Mascota).ThenInclude(m => m.Cliente)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var cleanSearch = searchTerm.Trim().ToLower();
            query = query.Where(v => v.Mascota.Nombre.ToLower().Contains(cleanSearch) || v.Vaccine.Name.ToLower().Contains(cleanSearch));
        }

        return await query
            .OrderBy(v => v.NextDoseDate)
            .ToPagedResultAsync(pageNumber, pageSize);
    }

    public async Task<List<PetVaccination>> GetDueForReminderAsync(DateOnly sevenDayTarget, DateOnly threeDayTarget)
        => await _context.PetVaccinations
            .Where(v => v.Status == "Applied" && v.NextDoseDate != null &&
                ((v.NextDoseDate == sevenDayTarget && v.ReminderSevenSentAt == null) ||
                 (v.NextDoseDate == threeDayTarget && v.ReminderThreeSentAt == null)))
            .Include(v => v.Vaccine)
            .Include(v => v.Mascota).ThenInclude(m => m.Cliente)
            .ToListAsync();

    public async Task<List<PetVaccination>> GetDueForDayBeforeReminderAsync(DateOnly dayBeforeTarget)
        => await _context.PetVaccinations
            .Where(v => v.Status == "Applied" && v.NextDoseDate == dayBeforeTarget && v.ReminderDayBeforeSentAt == null)
            .Include(v => v.Vaccine)
            .Include(v => v.Mascota).ThenInclude(m => m.Cliente)
            .ToListAsync();

    public async Task<Factura?> GetFacturaByConsultaIdAsync(int consultaId)
        => await _context.Facturas.FirstOrDefaultAsync(f => f.ConsultaId == consultaId);

    public void AddDetalleFactura(DetallesFactura detalle)
        => _context.DetallesFacturas.Add(detalle);

    public void UpdateFactura(Factura factura)
        => _context.Facturas.Update(factura);

    public async Task<bool> SaveChangesAsync()
        => await _context.SaveChangesAsync() > 0;
}
