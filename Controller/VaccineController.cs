using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Routes;

//Describe: Controlador del catálogo de vacunas: maestro de vacunas, esquemas por etapa y lotes.
namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VaccineController : ControllerBase
{
    private readonly IVaccineService _vaccineService;

    public VaccineController(IVaccineService vaccineService)
    {
        _vaccineService = vaccineService;
    }

    [HttpGet]
    public async Task<IActionResult> GetVaccines(
        [FromQuery] string? species,
        [FromQuery] bool? active,
        [FromQuery] string? searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _vaccineService.GetPagedAsync(pageNumber, pageSize, species, active, searchTerm);
            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseErrors.InternalServerError });
        }
    }

    [HttpGet(Endpoints.Vaccine.GetById)]
    public async Task<IActionResult> GetVaccineById(int id)
    {
        try
        {
            var vaccine = await _vaccineService.GetByIdAsync(id);
            if (vaccine == null) return NotFound(new { message = ResponseMessagesVaccination.VaccineNotFound });
            return Ok(vaccine);
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseErrors.InternalServerError });
        }
    }

    [HttpPost(Endpoints.Vaccine.Create)]
    public async Task<IActionResult> CreateVaccine([FromBody] VaccineCreateDTO dto)
    {
        try
        {
            var vaccine = await _vaccineService.CreateAsync(dto);
            return StatusCode(201, vaccine);
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

    [HttpPut(Endpoints.Vaccine.Update)]
    public async Task<IActionResult> UpdateVaccine(int id, [FromBody] VaccineCreateDTO dto)
    {
        try
        {
            var vaccine = await _vaccineService.UpdateAsync(id, dto);
            return Ok(vaccine);
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

    [HttpDelete(Endpoints.Vaccine.Delete)]
    public async Task<IActionResult> DeleteVaccine(int id)
    {
        try
        {
            await _vaccineService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseErrors.InternalServerError });
        }
    }

    [HttpPost(Endpoints.Vaccine.SchemeStages)]
    public async Task<IActionResult> AddSchemeStage(int id, [FromBody] VaccineSchemeStageCreateDTO dto)
    {
        try
        {
            var stage = await _vaccineService.AddSchemeStageAsync(id, dto);
            return StatusCode(201, stage);
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

    [HttpDelete(Endpoints.Vaccine.DeleteSchemeStage)]
    public async Task<IActionResult> RemoveSchemeStage(int id, int stageId)
    {
        try
        {
            await _vaccineService.RemoveSchemeStageAsync(id, stageId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseErrors.InternalServerError });
        }
    }

    [HttpGet(Endpoints.Vaccine.Batches)]
    public async Task<IActionResult> GetBatches(int id, [FromQuery] bool? active, [FromQuery] string? searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _vaccineService.GetBatchesPagedAsync(id, pageNumber, pageSize, active, searchTerm);
            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseErrors.InternalServerError });
        }
    }

    [HttpPost(Endpoints.Vaccine.CreateBatch)]
    public async Task<IActionResult> CreateBatch(int id, [FromBody] VaccineBatchCreateDTO dto)
    {
        try
        {
            var batch = await _vaccineService.AddBatchAsync(id, dto);
            return StatusCode(201, batch);
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
}
