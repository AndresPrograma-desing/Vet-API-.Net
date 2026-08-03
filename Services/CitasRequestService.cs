using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using vet_api_Net.Constants;
using vet_api_Net.Extensions;
using vet_api_Net.Infrastructure.Configuration;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Utilities;
using vet_api_Net.Models;
using vet_api_Net.Routes;

namespace vet_api_Net.Services;

public class CitasRequestService : ICitasRequestService
{
    private readonly ICitasRepository _citasRepository;
    private readonly IConfiguration _configuration;
    private readonly ApiSettingsOptions _apiSettings;
    private readonly IUserScopeResolver _userScopeResolver;

    public CitasRequestService(ICitasRepository citasRepository, IConfiguration configuration, IOptions<ApiSettingsOptions> apiSettingsOptions, IUserScopeResolver userScopeResolver)
    {
        _citasRepository = citasRepository;
        _configuration = configuration;
        _apiSettings = apiSettingsOptions.Value;
        _userScopeResolver = userScopeResolver;
    }

    public async Task<List<CitasRequestDTO>> GetAllCitasRequestsAsync(string? status = null, DateTime? date = null, bool allDates = false, string? currentUserRole = null, int? currentUserId = null, int? requestedDoctorId = null, int? requestedSecretariaId = null)
    {
        var (doctorId, secretariaId) = _userScopeResolver.ResolveDoctorSecretariaScope(currentUserRole, currentUserId, requestedDoctorId, requestedSecretariaId);

        try
        {
            var appointments = await _citasRepository.GetAllWithRelationsAsync();

            if (doctorId.HasValue)
                appointments = appointments.Where(a => a.DoctorId == doctorId.Value).ToList();

            if (secretariaId.HasValue)
                appointments = appointments.Where(a => a.SecretariaId == secretariaId.Value).ToList();
            var now = DateTime.Now;
            var anyExpired = false;

            foreach (var a in appointments)
            {
                var currentStatus = a.Estado?.Trim().ToLowerInvariant();
                if (currentStatus != Status.Cancelled &&
                    currentStatus != Status.Completed &&
                    currentStatus != Status.NotAssisted)
                {
                    var appointmentDateTime = new DateTime(
                        a.FechaCita.Year,
                        a.FechaCita.Month,
                        a.FechaCita.Day,
                        a.HoraCita.Hour,
                        a.HoraCita.Minute,
                        a.HoraCita.Second
                    );

                    if (appointmentDateTime < now)
                    {
                        a.Estado = Status.NotAssisted;
                        _citasRepository.Update(a);
                        anyExpired = true;
                    }
                }
            }

            if (anyExpired)
            {
                await _citasRepository.SaveChangesAsync();
            }


            if (!allDates)
            {
                DateTime targetDate = date?.Date ?? DateTime.Today;

                appointments = appointments.Where(a => a.FechaCita.Date == targetDate).ToList();
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                appointments = appointments.Where(a => a.Estado.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return appointments.Select(a =>
            {
                var dto = a.ToRequestDTO();
                dto.FechaCita = a.FechaCita.ToString(_apiSettings.DateFormat);
                return dto;
            }).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"{ResponseMessagesCitas.ErrorRequestingCitas} : {ex.Message}");
        }
    }
    public async Task<CitaDetalleDTO?> GetCitaRequestDetailsAsync(int id)
    {
        try
        {
            var cita = await _citasRepository.GetByIdWithRelationsAsync(id);
            if (cita == null) return null;

            var currentStatus = cita.Estado?.Trim().ToLowerInvariant();
            if (currentStatus != Status.Cancelled &&
                currentStatus != Status.Completed &&
                currentStatus != Status.NotAssisted)
            {
                var appointmentDateTime = new DateTime(
                    cita.FechaCita.Year,
                    cita.FechaCita.Month,
                    cita.FechaCita.Day,
                    cita.HoraCita.Hour,
                    cita.HoraCita.Minute,
                    cita.HoraCita.Second
                );

                if (appointmentDateTime < DateTime.Now)
                {
                    cita.Estado = Status.NotAssisted;
                    _citasRepository.Update(cita);
                    await _citasRepository.SaveChangesAsync();
                }
            }


            return new CitaDetalleDTO
            {
                Id = cita.Id,
                FechaCita = cita.FechaCita,
                HoraCita = cita.HoraCita.ToTimeSpan(),
                Motivo = cita.Motivo,
                TipoCita = cita.TipoCita ?? string.Empty,
                Estado = cita.Estado ?? string.Empty,
                Notas = cita.Notas,
                Mascota = cita.Mascota != null ? new MascotaResumenDTO
                {
                    Id = cita.Mascota.Id,
                    ClienteId = cita.Mascota.ClienteId,
                    Nombre = cita.Mascota.Nombre,
                    Especie = cita.Mascota.Especie,
                    Raza = cita.Mascota.Raza,
                    Sexo = cita.Mascota.Sexo,
                    FechaNacimiento = cita.Mascota.FechaNacimiento?.ToString(_apiSettings.DateFormat),
                    Peso = cita.Mascota.Peso?.ToString("0.00", CultureInfo.InvariantCulture),
                    IdenteficacionMascota = cita.Mascota.IdenteficacionMascota,
                    Cliente = cita.Mascota.Cliente != null ? new DetailsCitaDTO
                    {
                        Id = cita.Mascota.Cliente.Id,
                        Nombre = cita.Mascota.Cliente.Nombre,
                        Apellido = cita.Mascota.Cliente.Apellido,
                        Email = cita.Mascota.Cliente.Email,
                        Telefono = cita.Mascota.Cliente.Telefono,
                        Identificacion = cita.Mascota.Cliente.Identificacion
                    } : null!
                } : null!,
                Doctor = cita.Doctor != null ? new UsuarioResumenDTO
                {
                    Id = cita.Doctor.Id,
                    Nombre = cita.Doctor.Nombre,
                    Apellido = cita.Doctor.Apellido,
                    Rol = cita.Doctor.Rol
                } : null!,
                Secretaria = cita.Secretaria != null ? new UsuarioResumenDTO
                {
                    Id = cita.Secretaria.Id,
                    Nombre = cita.Secretaria.Nombre,
                    Apellido = cita.Secretaria.Apellido,
                    Rol = cita.Secretaria.Rol
                } : null,
                MetodoPagoId = cita.MetodoPagoId,
                MetodoPago = cita.MetodoPago?.Nombre
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"{ResponseMessagesCitas.ErrorRequestDetails} : {ex.Message}");
        }
    }

    public async Task<DeleteCitaDTO?> DeleteCitaAsync(int id)
    {
        try
        {
            var cita = await _citasRepository.GetByIdAsync(id);
            if (cita == null) return null;

            _citasRepository.Delete(cita);
            await _citasRepository.SaveChangesAsync();

            return new DeleteCitaDTO { Id = cita.Id };
        }
        catch (Exception ex)
        {
            throw new Exception($"{ResponseMessagesCitas.ErrorDeletingCita} : {ex.Message}");
        }
    }

    public async Task<CreateCitaDTO> CreateCitaAsync(CreateCitaDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (!TimeOnly.TryParse(dto.HoraCita, out TimeOnly hora))
        {
            if (!TimeOnly.TryParseExact(dto.HoraCita, new[] { _apiSettings.TimeFormat, "HH:mm" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out hora))
            {
                throw new ArgumentException(ResponseMessagesCitas.InvalidHoraCita);
            }
        }

        var allowedTypes = new[] { TypeConsultas.Consulta, TypeConsultas.Vacunacion, TypeConsultas.Cirugia, TypeConsultas.Emergencia, TypeConsultas.Seguimiento };
        var tipo = string.IsNullOrWhiteSpace(dto.TipoCita) ? TypeConsultas.Consulta : dto.TipoCita;
        if (!allowedTypes.Contains(tipo, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException($"{ResponseMessagesCitas.StatuNoValid} {string.Join(", ", allowedTypes)}");

        var allowedStates = new[] { Status.Programed, Status.Completed, Status.Cancelled, Status.NotAssisted, Status.InCurso };
        var estado = string.IsNullOrWhiteSpace(dto.Estado) ? Status.Programed : dto.Estado;
        if (!allowedStates.Contains(estado, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException($"{ResponseMessagesCitas.StatuNoValid} {string.Join(", ", allowedStates)}");

        var cita = new Cita
        {
            MascotaId = dto.MascotaId,
            DoctorId = dto.DoctorId,
            SecretariaId = dto.SecretariaId,
            FechaCita = dto.FechaCita?.Date ?? DateTime.Now.Date,
            HoraCita = hora,
            Motivo = dto.Motivo,
            TipoCita = tipo,
            Estado = estado,
            Notas = dto.Notas
        };

        await _citasRepository.AddAsync(cita);
        await _citasRepository.SaveChangesAsync();

        return dto;
    }

    public async Task<CitasRequestDTO?> UpdateCitaAsync(int id, UpdateCitaDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var cita = await _citasRepository.GetByIdWithRelationsAsync(id);
        if (cita == null) return null;

        if (!string.IsNullOrWhiteSpace(dto.HoraCita))
        {
            if (!TimeOnly.TryParse(dto.HoraCita, out TimeOnly hora))
            {
                if (!TimeOnly.TryParseExact(dto.HoraCita, new[] { _apiSettings.TimeFormat, "HH:mm", "HH:mm:ss", "h:mm tt", "hh:mm tt", "h:mm TT", "hh:mm TT" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out hora))
                {
                    throw new ArgumentException(ResponseMessagesCitas.InvalidHoraCita);
                }
            }
            cita.HoraCita = hora;
        }

        if (!string.IsNullOrWhiteSpace(dto.TipoCita))
        {
            var val = dto.TipoCita.Trim().ToLowerInvariant();
            if (val.Contains("consulta")) val = TypeConsultas.Consulta;
            else if (val.Contains("vacuna")) val = TypeConsultas.Vacunacion;
            else if (val.Contains("cirug")) val = TypeConsultas.Cirugia;
            else if (val.Contains("emerg")) val = TypeConsultas.Emergencia;
            else if (val.Contains("segui")) val = TypeConsultas.Seguimiento;

            var allowedTypes = new[] { TypeConsultas.Consulta, TypeConsultas.Vacunacion, TypeConsultas.Cirugia, TypeConsultas.Emergencia, TypeConsultas.Seguimiento };
            if (!allowedTypes.Contains(val, StringComparer.OrdinalIgnoreCase))
                throw new ArgumentException($"Tipo de cita no válido. Valores permitidos: {string.Join(", ", allowedTypes)}");
            cita.TipoCita = val;
        }

        if (!string.IsNullOrWhiteSpace(dto.Estado))
        {
            var allowedStates = new[] { Status.Programed, Status.Completed, Status.Cancelled, Status.NotAssisted, Status.InCurso, Status.Pending };
            if (!allowedStates.Contains(dto.Estado, StringComparer.OrdinalIgnoreCase))
                throw new ArgumentException($"{ResponseMessagesCitas.StatuNoValid} {string.Join(", ", allowedStates)}");
            cita.Estado = dto.Estado;
        }

        if (dto.MascotaId > 0) cita.MascotaId = dto.MascotaId;
        if (dto.DoctorId > 0) cita.DoctorId = dto.DoctorId;
        if (dto.SecretariaId.HasValue) cita.SecretariaId = dto.SecretariaId.Value;
        if (dto.FechaCita.HasValue) cita.FechaCita = dto.FechaCita.Value.Date;
        if (dto.Motivo != null) cita.Motivo = dto.Motivo;
        if (dto.Notas != null) cita.Notas = dto.Notas;

        _citasRepository.Update(cita);
        await _citasRepository.SaveChangesAsync();

        var updatedCita = await _citasRepository.GetByIdWithRelationsAsync(id);
        var resultDto = (updatedCita ?? cita).ToRequestDTO();
        resultDto.FechaCita = (updatedCita ?? cita).FechaCita.ToString(_apiSettings.DateFormat);
        return resultDto;
    }

    public async Task<StatusCitaRequestDTO> StatusCitaRequestAsync(int id)
    {
        var statusCita = await _citasRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException(ResponseMessagesCitas.CitaNotFound);

        if (statusCita.Estado == Status.InCursoII)
            throw new InvalidOperationException(ResponseMessagesCitas.ErrorModifyingCita);

        statusCita.Estado = Status.Completed;
        _citasRepository.Update(statusCita);
        await _citasRepository.SaveChangesAsync();

        return new StatusCitaRequestDTO { Estado = statusCita.Estado };
    }

    public async Task<StatusCitaRequestDTO?> UpdateCitaStatusAsync(int id, StatusCitaRequestDTO request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var cita = await _citasRepository.GetByIdAsync(id);
        if (cita == null) return null;

        var allowed = new[] { Status.Completed, Status.InCursoII, Status.Cancelled, Status.NotAssisted };
        if (string.IsNullOrWhiteSpace(request.Estado) || !allowed.Contains(request.Estado, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException($"{ResponseMessagesCitas.StatusValid}");

        cita.Estado = request.Estado;
        _citasRepository.Update(cita);
        await _citasRepository.SaveChangesAsync();

        return new StatusCitaRequestDTO { Estado = cita.Estado };
    }

    public async Task<List<CitasRequestDTO>> CurrentCitaAsync()
    {
        try
        {
            var citas = await _citasRepository.GetByStatusAsync(Status.NotAssisted);
            return citas.Select(c => c.ToRequestDTO()).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"{ResponseMessagesCitas.ErrorProcessingCita} : {ex.Message}");
        }
    }
    public async Task<List<NotificationCitaDTO>> NotificationCitaAsync(string? currentUserRole = null, int? currentUserId = null, int? requestedDoctorId = null, int? requestedSecretariaId = null)
    {
        var (doctorId, secretariaId) = _userScopeResolver.ResolveDoctorSecretariaScope(currentUserRole, currentUserId, requestedDoctorId, requestedSecretariaId);

        try
        {
            var ahora = DateTime.Now;
            var horaActual = TimeOnly.FromDateTime(ahora);

            return await _citasRepository.GetUpcomingNotificationsAsync(
                ahora.Date,
                horaActual,
                _apiSettings.DateFormat!,
                _apiSettings.TimeFormat!,
                doctorId,
                secretariaId
            );
        }
        catch (Exception ex)
        {
            throw new Exception($"{ResponseMessagesCitas.ErrorProcessingCita}: {ex.Message}");
        }
    }

       public Task<List<string>> GetCitaStatusesAsync()
    {
        var statuses = new List<string>
    {
        Status.Cancelled,
        Status.Completed,
        Status.InCurso,
        Status.NotAssisted,
        Status.Pending,
        Status.Programed

    };
        return Task.FromResult(statuses);
    }

    public Task<List<string>> GetCitaTypesAsync()
    {
        var types = new List<string>
        {
            TypeConsultas.Consulta,
            TypeConsultas.Vacunacion,
            TypeConsultas.Cirugia,
            TypeConsultas.Emergencia,
            TypeConsultas.Seguimiento,
            TypeConsultas.Desparasitacion
        };
        return Task.FromResult(types);
    }
}