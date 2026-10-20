using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DTOs;
using vet_api_Net.Constants;
using vet_api_Net.Routes;
using vet_api_Net.Interfaze.Services.Clients;

namespace vet_api_Net.Controllers;

//Describe: Controlador HTTP de las acciones de autoservicio del cliente autenticado: cancelar una cita propia, solicitar una cita nueva y solicitar la renovación de una receta.
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Cliente)]
public class ClientRequestsController : ControllerBase
{
    private readonly IClientRequestsService _clientRequestsService;

    public ClientRequestsController(IClientRequestsService clientRequestsService)
    {
        _clientRequestsService = clientRequestsService;
    }

    private int? GetClientId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(idClaim, out var id) ? id : null;
    }

    [HttpPost(Endpoints.ClientRequests.CancelCita)]
    public async Task<IActionResult> CancelCita([FromRoute] int citaId, [FromBody] ClientCancelCitaDTO request)
    {
        var clientId = GetClientId();
        if (clientId == null) return Unauthorized();

        try
        {
            await _clientRequestsService.CancelCitaAsync(clientId.Value, citaId, request?.Motivo ?? string.Empty);
            return Ok(new { success = true, message = ResponseMessagesClientRequests.CitaCancelledSuccess });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { success = false, error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { success = false, error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { success = false, error = ResponseErrors.InternalServerError });
        }
    }

    [HttpPost(Endpoints.ClientRequests.ConfirmCita)]
    public async Task<IActionResult> ConfirmCita([FromRoute] int citaId, [FromBody] ClientConfirmCitaDTO request)
    {
        var clientId = GetClientId();
        if (clientId == null) return Unauthorized();

        try
        {
            await _clientRequestsService.ConfirmCitaAsync(clientId.Value, citaId, request?.Mensaje);
            return Ok(new { success = true, message = ResponseMessagesClientRequests.CitaConfirmedSuccess });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { success = false, error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { success = false, error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { success = false, error = ResponseErrors.InternalServerError });
        }
    }

    [HttpPost(Endpoints.ClientRequests.RequestCita)]
    public async Task<IActionResult> RequestCita([FromBody] ClientRequestCitaDTO request)
    {
        var clientId = GetClientId();
        if (clientId == null) return Unauthorized();

        try
        {
            await _clientRequestsService.RequestCitaAsync(clientId.Value, request);
            return Ok(new { success = true, message = ResponseMessagesClientRequests.CitaRequestSentSuccess });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { success = false, error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { success = false, error = ResponseErrors.InternalServerError });
        }
    }

    [HttpPost(Endpoints.ClientRequests.RequestRecetaRenewal)]
    public async Task<IActionResult> RequestRecetaRenewal([FromRoute] int consultaId, [FromBody] ClientRequestRecetaRenewalDTO request)
    {
        var clientId = GetClientId();
        if (clientId == null) return Unauthorized();

        try
        {
            await _clientRequestsService.RequestRecetaRenewalAsync(clientId.Value, consultaId, request?.Nota ?? string.Empty);
            return Ok(new { success = true, message = ResponseMessagesClientRequests.RecetaRenewalRequestSentSuccess });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { success = false, error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { success = false, error = ResponseErrors.InternalServerError });
        }
    }
}
