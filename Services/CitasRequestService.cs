using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Models;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Extensions;
using vet_api_Net.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace vet_api_Net.Services;

public class CitasRequestService : ICitasRequestService
{
    private readonly ICitasRepository _citasRepository;
    private readonly IConfiguration _configuration;
    private readonly ApiSettingsOptions _apiSettings;

    public CitasRequestService(ICitasRepository citasRepository, IConfiguration configuration,IOptions<ApiSettingsOptions> apiSettingsOptions)
    {
        _citasRepository = citasRepository;
        _configuration = configuration;
        _apiSettings = apiSettingsOptions.Value;
    }

    public async Task<List<CitasRequestDTO>> GetAllCitasRequestsAsync()
    {


        try
        {
            var citas = await _citasRepository.GetAllWithRelationsAsync();

            return citas.Select(c =>
            {
                var dto = c.ToRequestDTO();

                dto.FechaCita = c.FechaCita.ToString(_apiSettings.DateFormat);

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
 public async Task<List<NotificationCitaDTO>> NotificationCitaAsync()
    {
        try
        {
            var ahora = DateTime.Now;
            var horaActual = TimeOnly.FromDateTime(ahora);

            return await _citasRepository.GetUpcomingNotificationsAsync(
                ahora.Date, 
                horaActual, 
                _apiSettings.DateFormat!, 
                _apiSettings.TimeFormat!
            );
        }
        catch (Exception ex)
        {
            throw new Exception($"{ResponseMessagesCitas.ErrorProcessingCita}: {ex.Message}");
        }
    }
}