using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Routes;

namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicioController : ControllerBase
{
    private readonly IServicioService _service;

    public ServicioController(IServicioService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ServicioListResponseDTO>> GetServicios(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? active = null,
        [FromQuery] string? searchTerm = null)
    {
        try
        {
            var result = await _service.GetServiciosAsync(pageNumber, pageSize, active, searchTerm);
            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseErrors.InternalServerError });
        }
    }

    [HttpGet(Endpoints.Servicio.GetById)]
    public async Task<ActionResult<ServicioDTO>> GetById(int id)
    {
        try
        {
            var servicio = await _service.GetByIdAsync(id);
            if (servicio == null) return NotFound(new { message = ResponseMessagesServicio.ServicioNotFound });
            return Ok(servicio);
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseErrors.InternalServerError });
        }
    }

    [HttpPost(Endpoints.Servicio.Create)]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<ServicioDTO>> Create([FromBody] ServicioCreateDTO dto)
    {
        try
        {
            var servicio = await _service.CreateAsync(dto);
            return StatusCode(201, servicio);
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

    [HttpPut(Endpoints.Servicio.Update)]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<ServicioDTO>> Update(int id, [FromBody] ServicioUpdateDTO dto)
    {
        try
        {
            var servicio = await _service.UpdateAsync(id, dto);
            return Ok(servicio);
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

    [HttpDelete(Endpoints.Servicio.Delete)]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
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
}
