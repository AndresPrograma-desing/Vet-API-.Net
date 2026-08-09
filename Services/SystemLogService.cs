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
            Accion = accion,
            TablaAfectada = tablaAfectada,
            RegistroId = registroId,
            DatosPrevios = datosPrevios,
            DatosNuevos = datosNuevos,
            UsuarioId = usuarioId,
            IpAddress = ipAddress,
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

    public async Task<List<LogsSistemaResponseDTO>> GetLogsAsync(int pageNumber, int pageSize)
    {
        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize = 10;

        var logs = await _repository.GetLogsAsync(pageNumber, pageSize);

        return logs.Select(l => new LogsSistemaResponseDTO
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
        }).ToList();
    }
}
