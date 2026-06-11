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

        var conflict = await _context.Citas.AnyAsync(c => c.DoctorId == dto.DoctorId && c.FechaCita.Date == fecha && c.HoraCita == hora);
        if (conflict) throw new InvalidOperationException(ResponseMessagesCitas.ExistingCitaConflict);

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