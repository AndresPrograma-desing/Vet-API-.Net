using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Models;
using vet_api_Net.Interfaze.Repositories;

//Describe: Implementación del repositorio para gestionar la persistencia y lectura de la Historia Clínica de mascotas y sus relaciones.
namespace vet_api_Net.Repositories;

public class HistoriaClinicaRepository : IHistoriaClinicaRepository
{
    private readonly AppDbContext _context;

    public HistoriaClinicaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<HistoriaClinica?> GetByMascotaIdAsync(int mascotaId)
        => await _context.HistoriasClinicas
            .OrderByDescending(hc => hc.Id)
            .FirstOrDefaultAsync(hc => hc.MascotaId == mascotaId);

    public async Task<List<HistoriaClinica>> GetAllByMascotaIdAsync(int mascotaId)
        => await _context.HistoriasClinicas
            .Where(hc => hc.MascotaId == mascotaId)
            .OrderByDescending(hc => hc.Id)
            .ToListAsync();

    public async Task<HistoriaClinica?> GetByIdAsync(int id)
        => await _context.HistoriasClinicas.FindAsync(id);

    public void Add(HistoriaClinica historiaClinica)
        => _context.HistoriasClinicas.Add(historiaClinica);

    public void Update(HistoriaClinica historiaClinica)
        => _context.HistoriasClinicas.Update(historiaClinica);

    public async Task<bool> SaveChangesAsync()
        => await _context.SaveChangesAsync() > 0;

    public async Task<Mascota?> GetMascotaWithDetailsAsync(int mascotaId)
        => await _context.Mascotas.FindAsync(mascotaId);

    public async Task<List<Consulta>> GetConsultasByMascotaIdAsync(int mascotaId)
        => await _context.Consultas
            .Where(c => c.MascotaId == mascotaId)
            .Include(c => c.Doctor)
            .OrderByDescending(c => c.FechaConsulta)
            .ToListAsync();

    public async Task<List<PetVaccination>> GetPetVaccinationsByMascotaIdAsync(int mascotaId)
        => await _context.PetVaccinations
            .Where(v => v.MascotaId == mascotaId)
            .Include(v => v.Vaccine)
            .Include(v => v.Doctor)
            .OrderByDescending(v => v.ApplicationDate)
            .ToListAsync();
}
