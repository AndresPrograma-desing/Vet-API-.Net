using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Models;
using vet_api_Net.Interfaces.Repositories;
using DTOs;

namespace vet_api_Net.Repositories;

public class CitasRepository : ICitasRepository
{
    private readonly AppDbContext _context;

    public CitasRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Cita>> GetAllWithRelationsAsync()
    {
        return await _context.Citas
            .Include(c => c.Mascota).ThenInclude(m => m.Cliente)
            .Include(c => c.Doctor)
            .Include(c => c.Secretaria)
            .Include(c => c.MetodoPago)
            .ToListAsync();
    }

    public async Task<Cita?> GetByIdWithRelationsAsync(int id)
    {
        return await _context.Citas
            .Include(c => c.Mascota).ThenInclude(m => m.Cliente)
            .Include(c => c.Doctor)
            .Include(c => c.Secretaria)
            .Include(c => c.MetodoPago)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Cita?> GetByIdAsync(int id)
    {
        return await _context.Citas.FindAsync(id);
    }

    public async Task<List<Cita>> GetByStatusAsync(string estado)
    {
        return await _context.Citas
            .Include(c => c.Mascota).ThenInclude(m => m.Cliente)
            .Include(c => c.Doctor)
            .Include(c => c.Secretaria)
            .Include(c => c.MetodoPago)
            .Where(c => c.Estado == estado)
            .ToListAsync();
    }

    public async Task<List<Cita>> GetByDateAsync(DateTime fecha)
    {
        return await _context.Citas
            .Include(c => c.Mascota).ThenInclude(m => m.Cliente)
            .Where(c => c.FechaCita.Date == fecha.Date)
            .ToListAsync();
    }
    public async Task<List<NotificationCitaDTO>> GetUpcomingNotificationsAsync(DateTime fecha, TimeOnly horaDesde, string dateFormat, string timeFormat)
    {
        return await _context.Citas
            .Where(c => c.FechaCita.Date == fecha.Date && c.HoraCita > horaDesde)
            .OrderBy(c => c.HoraCita)
            .Select(c => new NotificationCitaDTO
            {
                FechaCita = c.FechaCita.ToString(dateFormat),
                HoraCita = c.HoraCita.ToString(timeFormat),
                MascotaNombre = c.Mascota != null ? c.Mascota.Nombre : string.Empty,
                ClienteNombre = c.Mascota != null && c.Mascota.Cliente != null 
                    ? (c.Mascota.Cliente.Nombre + " " + c.Mascota.Cliente.Apellido).Trim() 
                    : string.Empty
            })
            .ToListAsync();
    }

    public async Task AddAsync(Cita cita) => await _context.Citas.AddAsync(cita);

    public void Update(Cita cita) => _context.Citas.Update(cita);

    public void Delete(Cita cita) => _context.Citas.Remove(cita);

    public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;

    public async Task<bool> AnyConflictAsync(int doctorId, DateTime fecha, TimeOnly hora)
    {
        return await _context.Citas.AnyAsync(c => c.DoctorId == doctorId && c.FechaCita.Date == fecha.Date && c.HoraCita == hora);
    }

    public async Task<MetodoPago?> GetMetodoPagoByNameAsync(string nombre)
    {
        var metodoNombre = nombre.Trim().ToLower();
        return await _context.MetodoPagos.FirstOrDefaultAsync(m => m.Nombre.ToLower() == metodoNombre);
    }

    public void AddMetodoPago(MetodoPago metodo)
    {
        _context.MetodoPagos.Add(metodo);
    }

    public async Task<Cita?> GetByIdWithMetodoPagoAsync(int id)
    {
        return await _context.Citas.Include(c => c.MetodoPago).FirstOrDefaultAsync(c => c.Id == id);
    }
}