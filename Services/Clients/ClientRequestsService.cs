using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Repositories.Clients;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Services.Clients;

namespace vet_api_Net.Services;

//Describe: Orquesta las acciones de autoservicio del cliente sobre citas y recetas (cancelar, solicitar, pedir renovación) y dispara la notificación interna correspondiente, sin modificar los servicios de empleados existentes.
public class ClientRequestsService : IClientRequestsService
{
    private static readonly string[] CancellableStates = { Status.Pending, Status.Programed };

    private readonly ICitasRepository _citasRepository;
    private readonly IConsultasRepository _consultasRepository;
    private readonly IClientRepository _clientRepository;
    private readonly INotificationService _notificationService;

    public ClientRequestsService(
        ICitasRepository citasRepository,
        IConsultasRepository consultasRepository,
        IClientRepository clientRepository,
        INotificationService notificationService)
    {
        _citasRepository = citasRepository;
        _consultasRepository = consultasRepository;
        _clientRepository = clientRepository;
        _notificationService = notificationService;
    }

    public async Task CancelCitaAsync(int clienteId, int citaId, string motivo)
    {
        if (string.IsNullOrWhiteSpace(motivo))
            throw new ArgumentException(ResponseMessagesClientRequests.RequiredMotivo);

        var cita = await _citasRepository.GetByIdWithRelationsAsync(citaId);
        if (cita == null)
            throw new KeyNotFoundException(ResponseMessagesClientRequests.CitaNotFound);

        if (cita.Mascota?.ClienteId != clienteId)
            throw new UnauthorizedAccessException(ResponseMessagesClientRequests.CitaNotOwned);

        if (string.IsNullOrWhiteSpace(cita.Estado) || !CancellableStates.Contains(cita.Estado, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException(ResponseMessagesClientRequests.CitaNotCancellable);

        cita.Estado = Status.Cancelled;
        _citasRepository.Update(cita);
        await _citasRepository.SaveChangesAsync();

        var clienteNombre = $"{cita.Mascota.Cliente?.Nombre} {cita.Mascota.Cliente?.Apellido}".Trim();
        var mascotaNombre = cita.Mascota.Nombre;

        await NotifyRolesAsync(
            new[] { Roles.Doctor, Roles.Secretaria },
            ResponseMessagesClientRequests.CancelNotificationTitle(mascotaNombre),
            ResponseMessagesClientRequests.CancelNotificationMessage(clienteNombre, mascotaNombre, motivo),
            "CitaCanceladaPorCliente",
            "citas");
    }

    public async Task ConfirmCitaAsync(int clienteId, int citaId, string? mensaje)
    {
        var cita = await _citasRepository.GetByIdWithRelationsAsync(citaId);
        if (cita == null)
            throw new KeyNotFoundException(ResponseMessagesClientRequests.CitaNotFound);

        if (cita.Mascota?.ClienteId != clienteId)
            throw new UnauthorizedAccessException(ResponseMessagesClientRequests.CitaNotOwned);

        if (string.IsNullOrWhiteSpace(cita.Estado) || !CancellableStates.Contains(cita.Estado, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException(ResponseMessagesClientRequests.CitaNotRespondable(cita.Estado ?? string.Empty));

        var clienteNombre = $"{cita.Mascota.Cliente?.Nombre} {cita.Mascota.Cliente?.Apellido}".Trim();
        var mascotaNombre = cita.Mascota.Nombre;

        await NotifyRolesAsync(
            new[] { Roles.Doctor, Roles.Secretaria },
            ResponseMessagesClientRequests.ConfirmNotificationTitle(mascotaNombre),
            ResponseMessagesClientRequests.ConfirmNotificationMessage(clienteNombre, mascotaNombre, mensaje ?? string.Empty),
            "CitaConfirmadaPorCliente",
            "citas");
    }

    public async Task RequestCitaAsync(int clienteId, ClientRequestCitaDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (string.IsNullOrWhiteSpace(dto.Motivo))
            throw new ArgumentException(ResponseMessagesClientRequests.RequiredMotivo);

        var mascotas = await _clientRepository.GetMascotasByClienteIdAsync(clienteId);
        var mascota = mascotas.FirstOrDefault(m => m.Id == dto.MascotaId);
        if (mascota == null)
            throw new UnauthorizedAccessException(ResponseMessagesClientRequests.MascotaNotOwned);

        if (dto.FechaPreferida.HasValue)
        {
            var fechaPreferida = dto.FechaPreferida.Value.Date;
            if (fechaPreferida < DateTime.Now.Date)
                throw new ArgumentException(ResponseMessagesClientRequests.InvalidFechaPreferida);

            if (fechaPreferida == DateTime.Now.Date
                && !string.IsNullOrWhiteSpace(dto.HoraPreferida)
                && TimeOnly.TryParse(dto.HoraPreferida, out var horaPreferida)
                && fechaPreferida.Add(horaPreferida.ToTimeSpan()) < DateTime.Now)
            {
                throw new ArgumentException(ResponseMessagesClientRequests.InvalidFechaPreferida);
            }
        }

        var cliente = await _clientRepository.GetByIdSimpleAsync(clienteId);
        var clienteNombre = cliente != null ? $"{cliente.Nombre} {cliente.Apellido}".Trim() : string.Empty;

        var preferencia = string.Empty;
        if (dto.FechaPreferida.HasValue || !string.IsNullOrWhiteSpace(dto.HoraPreferida))
        {
            var fechaTexto = dto.FechaPreferida.HasValue ? dto.FechaPreferida.Value.ToString("dd/MM/yyyy") : "";
            var horaTexto = dto.HoraPreferida ?? "";
            preferencia = $" Preferencia: {fechaTexto} {horaTexto}".TrimEnd();
        }

        await NotifyRolesAsync(
            new[] { Roles.Admin, Roles.Secretaria, Roles.Doctor },
            ResponseMessagesClientRequests.NewCitaNotificationTitle(mascota.Nombre),
            ResponseMessagesClientRequests.NewCitaNotificationMessage(clienteNombre, mascota.Nombre, dto.Motivo, preferencia),
            "SolicitudCitaCliente",
            "citas",
            referenciaId: clienteId);
    }

    public async Task RequestRecetaRenewalAsync(int clienteId, int consultaId, string nota)
    {
        if (string.IsNullOrWhiteSpace(nota))
            throw new ArgumentException(ResponseMessagesClientRequests.RequiredNota);

        var consulta = await _consultasRepository.GetConsultaByIdEntityAsync(consultaId);
        if (consulta == null)
            throw new KeyNotFoundException(ResponseMessagesClientRequests.ConsultaNotFound);

        var mascotas = await _clientRepository.GetMascotasByClienteIdAsync(clienteId);
        var mascota = mascotas.FirstOrDefault(m => m.Id == consulta.MascotaId);
        if (mascota == null)
            throw new UnauthorizedAccessException(ResponseMessagesClientRequests.MascotaNotOwned);

        var cliente = await _clientRepository.GetByIdSimpleAsync(clienteId);
        var clienteNombre = cliente != null ? $"{cliente.Nombre} {cliente.Apellido}".Trim() : string.Empty;
        var recetaOriginal = string.IsNullOrWhiteSpace(consulta.Receta) ? "sin receta registrada" : consulta.Receta;

        await NotifyRolesAsync(
            new[] { Roles.Admin, Roles.Secretaria, Roles.Doctor },
            ResponseMessagesClientRequests.RecetaRenewalNotificationTitle(mascota.Nombre),
            ResponseMessagesClientRequests.RecetaRenewalNotificationMessage(clienteNombre, mascota.Nombre, recetaOriginal, nota),
            "SolicitudRenovacionReceta",
            "consultas");
    }

    private async Task NotifyRolesAsync(string[] roles, string titulo, string mensaje, string tipo, string origen, int? referenciaId = null)
    {
        foreach (var rol in roles)
        {
            var dto = new vet_api_Net.DTOs.AlertNotificationDTO
            {
                Titulo = titulo,
                Mensaje = mensaje,
                Tipo = tipo,
                Prioridad = "1",
                Destino = rol,
                Origen = origen,
                ReferenciaId = referenciaId
            };

            try
            {
                // creatorId: null — quien origina esto es un Cliente, no un Usuario, y DestinatarioId
                // tiene FK a usuarios.id; pasar un id falso viola esa FK. null evita el problema por
                // completo (el aviso opcional de "visto" entre empleados no aplica aquí de todas formas).
                await _notificationService.CreateNotificationAsync(dto, null);
            }
            catch (InvalidOperationException)
            {
                // Duplicado reciente (misma ventana de 5 min para el mismo título): no es un error fatal.
            }
        }
    }
}
