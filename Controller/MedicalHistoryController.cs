using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DTOs;
using vet_api_Net.Constants;
using vet_api_Net.Routes;
using vet_api_Net.Interfaze.Services;

//Describe: Controlador HTTP que expone los endpoints para interactuar con la Historia Clínica de mascotas y su análisis inteligente.
namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicalHistoryController : ControllerBase
{
    private readonly IHistoriaClinicaService _historiaClinicaService;

    public MedicalHistoryController(IHistoriaClinicaService historiaClinicaService)
    {
        _historiaClinicaService = historiaClinicaService;
    }

    [HttpGet(Endpoints.HistoriaClinica.GetByMascotaId)]
    public async Task<ActionResult<HistoriaClinicaResponseDTO>> GetByMascotaId([FromRoute] int mascotaId)
    {
        try
        {
            var result = await _historiaClinicaService.GetHistoriaClinicaByMascotaIdAsync(mascotaId);
            if (result == null)
            {
                return NotFound(new { message = ResponseMessagesHistoriaClinica.HistoriaClinicaNotFound });
            }
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
            return StatusCode(503, new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseErrors.InternalServerError });
        }
    }

    [HttpPatch(Endpoints.HistoriaClinica.UpdateNotes)]
    public async Task<ActionResult<HistoriaClinicaResponseDTO>> UpdateNotes([FromRoute] int id, [FromBody] UpdateHistoriaClinicaDTO dto)
    {
        try
        {
            var result = await _historiaClinicaService.UpdateNotasClinicasAsync(id, dto);
            if (result == null)
            {
                return NotFound(new { message = ResponseMessagesHistoriaClinica.HistoriaClinicaNotFound });
            }
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
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseErrors.InternalServerError });
        }
    }

    [HttpPost(Endpoints.HistoriaClinica.RefreshIa)]
    public async Task<ActionResult<HistoriaClinicaResponseDTO>> RefreshIa([FromRoute] int mascotaId)
    {
        try
        {
            var result = await _historiaClinicaService.RefrescarAnalisisIaAsync(mascotaId);
            if (result == null)
            {
                return NotFound(new { message = ResponseMessagesHistoriaClinica.HistoriaClinicaNotFound });
            }
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
            return StatusCode(503, new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseErrors.InternalServerError });
        }
    }
}
