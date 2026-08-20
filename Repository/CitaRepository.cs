using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTOs;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Constants;
using vet_api_Net.Data;
using vet_api_Net.Extensions;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Models;

namespace vet_api_Net.Repositories;

public class CitasRepository : ICitasRepository
{
    private readonly AppDbContext _context;

    public CitasRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Cita>> GetActiveForExpirationCheckAsync(int? doctorId, int? secretariaId)
    {
        var query = _context.Citas
            .Where(c => c.Estado != Status.Cancelled
                     && c.Estado != Status.Completed
                     && c.Estado != Status.NotAssisted);

        if (doctorId.HasValue)
            query = query.Where(c => c.DoctorId == doctorId.Value);

        if (secretariaId.HasValue)
            query = query.Where(c => c.SecretariaId == secretariaId.Value);

        return await query.ToListAsync();
    }

    public async Task<(List<Cita> Items, int TotalCount)> GetPagedWithRelationsAsync(int? doctorId, int? secretariaId, DateTime? date, bool allDates, string? status, int pageNumber, int pageSize)
    {
        var query = _context.Citas
            .Include(c => c.Mascota).ThenInclude(m => m.Cliente)
            .Include(c => c.Doctor)
            .Include(c => c.Secretaria)
            .Include(c => c.MetodoPago)
            .AsQueryable();

        if (doctorId.HasValue)
            query = query.Where(c => c.DoctorId == doctorId.Value);

        if (secretariaId.HasValue)
            query = query.Where(c => c.SecretariaId == secretariaId.Value);

        if (!allDates)
        {
            var targetDate = (date ?? DateTime.Today).Date;
            query = query.Where(c => c.FechaCita.Date == targetDate);
        }

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(c => c.Estado != null && c.Estado.ToLower() == status.ToLower());

        return await query
            .OrderByDescending(c => c.FechaCita)
            .ThenByDescending(c => c.HoraCita)
            .ToPagedResultAsync(pageNumber, pageSize);
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

    public async Task<List<Cita>> GetByClienteIdAsync(int clienteId)
    {
        return await _context.Citas
            .Include(c => c.Mascota)
            .Where(c => c.Mascota.ClienteId == clienteId)
            .OrderByDescending(c => c.FechaCita)
            .ToListAsync();
    }
   public async Task<List<NotificationCitaDTO>> GetUpcomingNotificationsAsync(DateTime fecha, TimeOnly horaDesde, string dateFormat, string timeFormat, int? doctorId = null, int? secretariaId = null)
{
    var startDate = fecha.Date;

    var query = _context.Citas.AsNoTracking()
        .Where(c => c.FechaCita >= startDate 
                 && c.FechaCita < startDate.AddDays(1)
                 && c.HoraCita > horaDesde
                 && c.Estado != Status.Completed
                 && c.Estado != Status.Cancelled);

    if (doctorId.HasValue)
        query = query.Where(c => c.DoctorId == doctorId.Value);

    if (secretariaId.HasValue)
        query = query.Where(c => c.SecretariaId == secretariaId.Value);

    var citasRaw = await query
        .OrderBy(c => c.HoraCita)
        .Select(c => new
        {
            c.FechaCita,
            c.HoraCita,
            MascotaNombre = c.Mascota != null ? c.Mascota.Nombre : string.Empty,
            ClienteNombre = c.Mascota != null && c.Mascota.Cliente != null
                ? (c.Mascota.Cliente.Nombre + " " + c.Mascota.Cliente.Apellido).Trim()
                : string.Empty
        })
        .ToListAsync();

    return citasRaw.Select(c => new NotificationCitaDTO
    {
        FechaCita = c.FechaCita.ToString(dateFormat),
        HoraCita = c.HoraCita.ToString(timeFormat),
        MascotaNombre = c.MascotaNombre,
        ClienteNombre = c.ClienteNombre
    }).ToList();
}

    public async Task AddAsync(Cita cita) => await _context.Citas.AddAsync(cita);

    public void Update(Cita cita) => _context.Citas.Update(cita);

    public void Delete(Cita cita) => _context.Citas.Remove(cita);

    public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;

    public Task<List<string>> GetCitaStatusesAsync()
    {
        var statuses = new List<string>
    {
        Status.Cancelled,
        Status.Completed,
        Status.InCurso,
        Status.NotAssisted,
        Status.Pending,
        Status.NotAssisted

    };
        return Task.FromResult(statuses);
    }

    // public async Task<Cita?> UpdateCitaAsync(int id)
    // {
        
    //     return await _context.Citas.FindAsync(id);
    // }
}