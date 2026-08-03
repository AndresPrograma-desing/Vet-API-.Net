using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DTOs;
using vet_api_Net.Routes;

using vet_api_Net.Interfaze.Services;
using vet_api_Net.Constants;

namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConsultasController : ControllerBase
{
    private readonly IConsultasService _consultasService;

    public ConsultasController(IConsultasService consultasService)
    {
        _consultasService = consultasService;
    }

    [HttpPost(Endpoints.Consultas.Create)]
 [HttpPost]
public async Task<ActionResult<ConsultaRequestDTO>> Create([FromBody] CreateConsultaDTO dto)
{
    try
    {
        var result = await _consultasService.CreateConsultaAsync(dto);
        return StatusCode(201, result);
    }
    catch (ArgumentNullException ex)
    {
        return BadRequest(new { message = ex.Message });
    }
    catch (ArgumentException ex)  
    {
        return BadRequest(new { message = ex.Message });
    }
    catch (KeyNotFoundException ex)  
    {
        return NotFound(new { message = ex.Message });
    }
    catch (Exception ex)  
    {
        return StatusCode(500, new { message = ResponseErrors.InternalServerError, details = ex.Message });
    }
}

    [HttpGet(Endpoints.Consultas.GetById)]
    public async Task<ActionResult<ConsultaRequestDTO>> GetById([FromRoute] int id)
    {
        try
        {
            var result = await _consultasService.GetConsultaByIdAsync(id);
            if (result == null) return NotFound();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ResponseErrors.InternalServerError, details = ex.Message});
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ConsultaRequestDTO>>> GetAll(
        [FromQuery] int? doctorId = null,
        [FromQuery] int? secretariaId = null)
    {
        try
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            int? currentUserId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var parsedId) ? parsedId : null;

            var result = await _consultasService.GetConsultasAsync(role, currentUserId, doctorId, secretariaId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new {message = ResponseErrors.InternalServerError, details = ex.Message });
        }
    }

    [HttpPatch(Endpoints.Consultas.UpdateReceta)]
    public async Task<ActionResult<ConsultaRequestDTO>> UpdateReceta([FromRoute] int id, [FromBody] UpdateConsultaRecetaDTO dto)
    {
        try
        {
            var result = await _consultasService.UpdateRecetaAsync(id, dto);
            return Ok(result);
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ResponseMessagesConsultas.ErrorCreated, details = ex.Message });
        }
    }
}