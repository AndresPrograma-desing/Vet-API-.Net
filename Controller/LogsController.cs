using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Routes;

namespace vet_api_Net.Controller;

//Describe: Controlador para acceder a los logs del sistema registrados por auditoría y errores.
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class LogsController : ControllerBase
{
    private readonly ISystemLogService _logService;

    public LogsController(ISystemLogService logService)
    {
        _logService = logService;
    }

    [HttpGet(Endpoints.Logs.GetLogs)]
    public async Task<ActionResult<List<LogsSistemaResponseDTO>>> GetLogs(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _logService.GetLogsAsync(pageNumber, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ResponseMessagesLogs.ErrorLoadingLogs, details = ex.Message });
        }
    }
}
