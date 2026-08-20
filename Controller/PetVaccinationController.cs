using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Routes;

//Describe: Controlador del flujo de aplicación de vacunas a mascotas: semaforización, registro, postergación, envío a factura, carnet e historial, y panel interno de refuerzos pendientes.
namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PetVaccinationController : ControllerBase
{
    private readonly IPetVaccinationService _petVaccinationService;

    public PetVaccinationController(IPetVaccinationService petVaccinationService)
    {
        _petVaccinationService = petVaccinationService;
    }

    [HttpGet(Endpoints.PetVaccination.StatusByMascota)]
    public async Task<IActionResult> GetStatus(int mascotaId)
    {
        try
        {
            var status = await _petVaccinationService.GetVaccinationStatusForMascotaAsync(mascotaId);
            return Ok(status);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet(Endpoints.PetVaccination.HistoryByMascota)]
    public async Task<IActionResult> GetHistory(int mascotaId, [FromQuery] string? status)
    {
        try
        {
            var history = await _petVaccinationService.GetHistoryForMascotaAsync(mascotaId, status);
            return Ok(history);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost(Endpoints.PetVaccination.Register)]
    public async Task<IActionResult> RegisterApplication([FromBody] RegisterPetVaccinationDTO dto)
    {
        try
        {
            var result = await _petVaccinationService.RegisterApplicationAsync(dto);
            return StatusCode(201, result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPatch(Endpoints.PetVaccination.Postpone)]
    public async Task<IActionResult> Postpone(int id, [FromBody] PostponeVaccinationDTO dto)
    {
        try
        {
            var result = await _petVaccinationService.PostponeAsync(id, dto);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost(Endpoints.PetVaccination.SendToInvoice)]
    public async Task<IActionResult> SendToInvoice(int id)
    {
        try
        {
            var result = await _petVaccinationService.SendToInvoiceAsync(id);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet(Endpoints.PetVaccination.PendingThisWeek)]
    public async Task<IActionResult> GetPendingThisWeek(
        [FromQuery] string? searchTerm, 
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _petVaccinationService.GetPendingThisWeekAsync(pageNumber, pageSize, searchTerm);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
