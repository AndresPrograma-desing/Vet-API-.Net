using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DTOs;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Routes;

//Describe: Controlador para acceder a los eventos de procesamiento de workers (errores/éxitos con reintentos), separados del log de auditoría HTTP general.
namespace vet_api_Net.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class WorkerProcessEventsController(IWorkerProcessEventService workerProcessEventService) : ControllerBase
{
    private readonly IWorkerProcessEventService _workerProcessEventService = workerProcessEventService;

    [HttpGet(Endpoints.WorkerProcessEvents.GetEvents)]
    public async Task<ActionResult<ProcessEventsListResponseDTO>> GetEvents(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? typeEvent = null,
        [FromQuery] bool? isResolved = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var result = await _workerProcessEventService.GetPagedAsync(
                pageNumber, pageSize, typeEvent, isResolved, fromDate, toDate);
            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseMessagesWorkerProcessEvents.ErrorLoadingEvents });
        }
    }
}
