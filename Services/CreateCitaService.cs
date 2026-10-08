using System;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Models;
using vet_api_Net.Constants;

namespace vet_api_Net.Services;

public class CreateCitaService : ICreateCitaService
{
    private readonly ICitasRepository _citasRepository;
    private readonly IPetsRepository _petsRepository;
    private readonly IUsersRepository _usersRepository;

    public CreateCitaService(ICitasRepository citasRepository, IPetsRepository petsRepository, IUsersRepository usersRepository)
    {
        _citasRepository = citasRepository;
        _petsRepository = petsRepository;
        _usersRepository = usersRepository;
    }

    public async Task<Cita> CreateCitaAsync(CreateCitaDTO dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        if (dto.MascotaId <= 0) throw new ArgumentException(ResponseMessagesCitas.InvalidMascotaId);
        if (dto.DoctorId <= 0) throw new ArgumentException(ResponseMessagesUsers.DoctorNotFound);
        if (string.IsNullOrWhiteSpace(dto.HoraCita)) throw new ArgumentException(ResponseMessagesCitas.RequiredHoraCita);

        if (!await _petsRepository.ExistsAsync(dto.MascotaId)) throw new KeyNotFoundException(ResponseMessagesCitas.MascotaNotFound);

        var doctor = await _usersRepository.GetByIdAsync(dto.DoctorId);
        if (doctor == null) throw new KeyNotFoundException(ResponseMessagesUsers.DoctorNotFound);

        if (dto.SecretariaId.HasValue)
        {
            var sec = await _usersRepository.GetByIdAsync(dto.SecretariaId.Value);
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
        var citasDelDia = await _citasRepository.GetByDoctorAndDateAsync(dto.DoctorId, fecha);

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
                    throw new InvalidOperationException(ResponseMessagesCitas.NotAvailableSlots);
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
            var metodo = await _citasRepository.GetPaymentMethodByNameAsync(metodoNombre);
            if (metodo == null)
            {
                metodo = new MetodoPago
                {
                    Nombre = metodoNombre,
                    Creado = DateTime.Now,
                    Actualizado = DateTime.Now
                };
                _citasRepository.AddPaymentMethod(metodo);
                await _citasRepository.SaveChangesAsync();
            }

            cita.MetodoPagoId = metodo.Id;
        }

        await _citasRepository.AddAsync(cita);
        await _citasRepository.SaveChangesAsync();

        var citaRecargada = await _citasRepository.GetByIdWithPaymentMethodAsync(cita.Id);
        return citaRecargada ?? cita;
    }
}