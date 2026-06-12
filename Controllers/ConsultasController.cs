using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DTOs;
using vet_api_Net.Routes;

using vet_api_Net.Interfaces.Services;

namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConsultasController : ControllerBase
{
    private readonly IConsultasService _consultasService;

    public ConsultasController(IConsultasService consultasService)
    {
        _consultasService = consultasService;
    }

    [HttpPost(Endpoints.Consultas.Create)]
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
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error handling internal records deployment.", details = ex.Message });
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
            return StatusCode(500, new { message = "Error retrieving specific record description.", details = ex.Message });
        }
    }
}