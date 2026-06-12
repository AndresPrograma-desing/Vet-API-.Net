using System;
using vet_api_Net.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using DTOs;
using vet_api_Net.Constants;
using vet_api_Net.Routes;

namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]

public class CitasController : ControllerBase
{
    private readonly ICitasRequestService _citasRequestService;

    public CitasController(ICitasRequestService citasRequestService)
    {
        _citasRequestService = citasRequestService;
    }

    [HttpGet(Endpoints.Citas.GetAllRequests)]
    public async Task<ActionResult<List<CitasRequestDTO>>> GetAllCitasRequests()
    {
        try
        {
            var citasRequests = await _citasRequestService.GetAllCitasRequestsAsync();
            return Ok(citasRequests);
        }
        catch (Exception)
        {
           
            return StatusCode(500, new { error = ResponseMessagesCitas.ErrorInterno });
        }
    }
 
    [HttpDelete(Endpoints.Citas.Delete)]
    public async Task<IActionResult> DeleteCita(int id)
    {
        try
        {
            var deleteCita = await _citasRequestService.DeleteCitaAsync(id);
            if(deleteCita == null)
            {
                return NotFound(new {message = ResponseMessagesCitas.CitaNotFound});

            }
            return Ok(deleteCita);
        }catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

  [HttpPatch(Endpoints.Citas.Finalizar)]
    public async Task<IActionResult> GetCitaStatus(int id)
    {
        try
        {
            var status = await _citasRequestService.StatusCitaRequestAsync(id);
            return Ok(status);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
}
    }

    [HttpPatch(Endpoints.Citas.NewStatus)]

    public async Task<IActionResult> StatusChamge(int id)
    {
        try{
        var status = await _citasRequestService.StatusCitaRequestAsync(id);
        return Ok(status);
        }catch(Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
    [HttpPatch(Endpoints.Citas.Status)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] StatusCitaRequestDTO request)
    {
        try
        {
            var status = await _citasRequestService.UpdateCitaStatusAsync(id, request);
            if (status == null) return NotFound(new { message = ResponseMessagesCitas.CitaNotFound });
            return Ok(status);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
//    [HttpGet("Unassigned")]
//     public async Task<ActionResult<List<CitasRequestDTO>>> CurrentCitaAsync()
//     {
//         try
//         {
//             var currentCitas = await _citasRequestService.CurrentCitaAsync();
//             if (currentCitas == null || !currentCitas.Any())
//                 return NotFound(new { message = "No hay citas no asistidas" });
//             return Ok(currentCitas);
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine(ex.Message);
//             return StatusCode(500, new { error = "Error interno al obtener citas no asistidas" });
//         }
//     }

[HttpGet(Endpoints.Citas.CitaToday)]
public async Task<ActionResult<List<CitasRequestDTO>>> GetTodayNotifications()
{
    try
    {
        var result = await _citasRequestService.NotificationCitaAsync();

        if (result.Count == 0)
        {
            return Ok(new { message = ResponseMessagesCitas.NotCitasToday, data = result });
        }

        return Ok(result);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { error = ex.Message });
    }
}
}