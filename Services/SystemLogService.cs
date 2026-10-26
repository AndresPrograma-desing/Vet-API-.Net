using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTOs;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Models;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;

namespace vet_api_Net.Services;

//Describe: Servicio para gestionar los logs del sistema, permitiendo su registro y consulta paginada.
public class SystemLogService : ISystemLogService
{
    private readonly ILogsSistemaRepository _repository;

    public SystemLogService(ILogsSistemaRepository repository)
    {
        _repository = repository;
    }

    public async Task RegisterLogAsync(
        string accion, 
        string? tablaAfectada = null, 
        int? registroId = null, 
        string? datosPrevios = null, 
        string? datosNuevos = null, 
        int? usuarioId = null,
        string? ipAddress = null,
        string? userAgent = null)
    {
        var log = new LogsSistema
        {
            // accion/tabla_afectada/ip_address tienen límite varchar en BD (100/50/45); se truncan
            // aquí para que un mensaje de error largo no tumbe también el registro del log.
            Accion = Truncate(accion, 100),
            TablaAfectada = Truncate(tablaAfectada, 50),
            RegistroId = registroId,
            DatosPrevios = datosPrevios,
            DatosNuevos = datosNuevos,
            UsuarioId = usuarioId,
            IpAddress = Truncate(ipAddress, 45),
            UserAgent = userAgent,
            CreatedAt = DateTime.Now
        };

        await _repository.AddLogAsync(log);

        try
        {
            await _repository.SaveChangesAsync();
        }
        catch (DbUpdateException) when (log.UsuarioId != null)
        {
            log.UsuarioId = null;
            await _repository.SaveChangesAsync();
        }
    }

    private static string? Truncate(string? value, int maxLength)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength) return value;
        return value.Substring(0, maxLength);
    }

    public async Task<LogsSistemaListResponseDTO> GetLogsAsync(
        int pageNumber,
        int pageSize,
        int? userId = null,
        string? action = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null)
    {
        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize = 10;

        var (logs, totalCount) = await _repository.GetLogsAsync(pageNumber, pageSize, userId, action, dateFrom, dateTo);

        return new LogsSistemaListResponseDTO
        {
            Items = logs.Select(l => new LogsSistemaResponseDTO
            {
                Id = l.Id,
                UsuarioId = l.UsuarioId,
                UsuarioNombre = l.Usuario != null ? $"{l.Usuario.Nombre} {l.Usuario.Apellido}".Trim() : null,
                Accion = l.Accion,
                TablaAfectada = l.TablaAfectada,
                RegistroId = l.RegistroId,
                DatosPrevios = l.DatosPrevios,
                DatosNuevos = l.DatosNuevos,
                IpAddress = l.IpAddress,
                UserAgent = l.UserAgent,
                CreatedAt = l.CreatedAt
            }).ToList(),
            TotalCount = totalCount
        };
    }
}