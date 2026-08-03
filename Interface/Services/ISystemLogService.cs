using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;

namespace vet_api_Net.Interfaze.Services;

public interface ISystemLogService
{
    Task RegisterLogAsync(
        string accion, 
        string? tablaAfectada = null, 
        int? registroId = null, 
        string? datosPrevios = null, 
        string? datosNuevos = null, 
        int? usuarioId = null,
        string? ipAddress = null,
        string? userAgent = null);

    Task<List<LogsSistemaResponseDTO>> GetLogsAsync(int pageNumber, int pageSize);
}
