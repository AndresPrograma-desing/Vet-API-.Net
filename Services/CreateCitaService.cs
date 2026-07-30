using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DTOs;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Data;
using vet_api_Net.Models;
using vet_api_Net.Constants;

namespace vet_api_Net.Services;

public class CreateCitaService : ICreateCitaService
{
    private readonly AppDbContext _context;

    public CreateCitaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Cita> CreateCitaAsync(CreateCitaDTO dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        if (dto.MascotaId <= 0) throw new ArgumentException(ResponseMessagesCitas.InvalidMascotaId);
        if (dto.DoctorId <= 0) throw new ArgumentException(ResponseMessagesUsers.DoctorNotFound);
        if (string.IsNullOrWhiteSpace(dto.HoraCita)) throw new ArgumentException(ResponseMessagesCitas.RequiredHoraCita);

        var mascota = await _context.Mascotas.FindAsync(dto.MascotaId);
        if (mascota == null) throw new KeyNotFoundException(ResponseMessagesCitas.MascotaNotFound);

        var doctor = await _context.Usuarios.FindAsync(dto.DoctorId);
        if (doctor == null) throw new KeyNotFoundException(ResponseMessagesUsers.DoctorNotFound);

        if (dto.SecretariaId.HasValue)
        {
            var sec = await _context.Usuarios.FindAsync(dto.SecretariaId.Value);
            if (sec == null) throw new KeyNotFoundException(ResponseMessagesUsers.SecretarialNotFound);
        }

        TimeOnly hora;
        try
        {
            hora = TimeOnly.Parse(dto.HoraCita);
        }
        catch (Exception)
        {
            throw new ArgumentException(ResponseMessagesCitas.InvalidHoraCita);
        }

        // --- MANEJO ROBUSTO DE LA FECHA ---
        DateTime fecha;
        if (dto.FechaCita.HasValue)
        {
            fecha = dto.FechaCita.Value.Date;
        }
        else
        {
            fecha = DateTime.Now.Date;
        }

        // --- EVITAR SOLAPAMIENTOS DE RANGOS DE 30 MINUTOS ---
        var citasDelDia = await _context.Citas
            .Where(c => c.DoctorId == dto.DoctorId && c.FechaCita.Date == fecha)
            .ToListAsync();

        var requestedStart = fecha.Add(hora.ToTimeSpan());
        var requestedEnd = requestedStart.AddMinutes(30);

        var citasConflictivas = citasDelDia
            .Where(c => 
                !string.Equals(c.Estado, Status.Cancelled, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(c.Estado, Status.Completed, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(c.Estado, Status.NotAssisted, StringComparison.OrdinalIgnoreCase))
            .Where(c => 
            {
                var existingEnd = c.FechaCita.Date.Add(c.HoraCita.ToTimeSpan()).AddMinutes(30);
                return existingEnd > DateTime.Now;
            })
            .ToList();

        var hasConflict = false;
        foreach (var existingCita in citasConflictivas)
        {
            var existingStart = existingCita.FechaCita.Date.Add(existingCita.HoraCita.ToTimeSpan());
            var existingEnd = existingStart.AddMinutes(30);

            if (requestedStart < existingEnd && requestedEnd > existingStart)
            {
                hasConflict = true;
                break;
            }
        }

        if (hasConflict)
        {
            if (dto.Autoagendar == true)
            {
                var standardHours = new List<string>
                {
                    "08:00", "08:30", "09:00", "09:30", "10:00", "10:30",
                    "11:00", "11:30", "12:00", "12:30", "13:00", "13:30",
                    "14:00", "14:30", "15:00", "15:30", "16:00", "16:30",
                    "17:00", "17:30"
                };

                var foundFreeSlot = false;
                var parsedHours = standardHours
                    .Select(h => TimeOnly.ParseExact(h, "HH:mm"))
                    .Where(t => t >= hora)
                    .OrderBy(t => t)
                    .ToList();

                foreach (var candidateTime in parsedHours)
                {
                    var candidateStart = fecha.Add(candidateTime.ToTimeSpan());
                    var candidateEnd = candidateStart.AddMinutes(30);

                    var isCandidateConflict = false;
                    foreach (var existingCita in citasConflictivas)
                    {
                        var existingStart = existingCita.FechaCita.Date.Add(existingCita.HoraCita.ToTimeSpan());
                        var existingEnd = existingStart.AddMinutes(30);

                        if (candidateStart < existingEnd && candidateEnd > existingStart)
                        {
                            isCandidateConflict = true;
                            break;
                        }
                    }

                    if (!isCandidateConflict)
                    {
                        hora = candidateTime;
                        foundFreeSlot = true;
                        break;
                    }
                }

                if (!foundFreeSlot)
                {
                    throw new InvalidOperationException("No se encontró ningún horario disponible para autoagendar este día.");
                }
            }
            else
            {
                throw new InvalidOperationException(ResponseMessagesCitas.ExistingCitaConflict);
            }
        }

        var cita = new Cita
        {
            MascotaId = dto.MascotaId,
            DoctorId = dto.DoctorId,
            SecretariaId = dto.SecretariaId,
            FechaCita = fecha,
            HoraCita = hora,
            Motivo = dto.Motivo,
            TipoCita = dto.TipoCita ?? TypeConsultas.Consulta,
            Estado = dto.Estado ?? Status.Programed,
            Notas = dto.Notas
        };
 
        if (!string.IsNullOrWhiteSpace(dto.MetodoPago))
        {
            var metodoNombre = dto.MetodoPago!.Trim();
            var metodo = await _context.MetodoPagos.FirstOrDefaultAsync(m => m.Nombre.ToLower() == metodoNombre.ToLower());
            if (metodo == null)
            {
                metodo = new MetodoPago
                {
                    Nombre = metodoNombre,
                    Creado = DateTime.Now,
                    Actualizado = DateTime.Now
                };
                _context.MetodoPagos.Add(metodo);
                await _context.SaveChangesAsync();
            }

            cita.MetodoPagoId = metodo.Id;
        }

        _context.Citas.Add(cita);
        await _context.SaveChangesAsync();
 
        var citaRecargada = await _context.Citas.Include(c => c.MetodoPago).FirstOrDefaultAsync(c => c.Id == cita.Id);
        return citaRecargada ?? cita;
    }
}