using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Routes;

//Describe: Controlador de administración de credenciales de integraciones externas (ej. API keys de Gemini) por proveedor.
namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IntegrationCredentialController : ControllerBase
{
    private readonly IIntegrationCredentialService _service;

    public IntegrationCredentialController(IIntegrationCredentialService service)
    {
        _service = service;
    }

    [HttpGet(Endpoints.IntegrationCredential.GetByProvider)]
    public async Task<IActionResult> GetByProvider(string provider)
    {
        try
        {
            var result = await _service.GetByProviderAsync(provider);
            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseErrors.InternalServerError });
        }
    }

    [HttpPost(Endpoints.IntegrationCredential.Create)]
    public async Task<IActionResult> Create([FromBody] CreateIntegrationCredentialDTO dto)
    {
        try
        {
            var result = await _service.CreateAsync(dto);
            return StatusCode(201, result);
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

    [HttpPut(Endpoints.IntegrationCredential.Update)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateIntegrationCredentialDTO dto)
    {
        try
        {
            var result = await _service.UpdateAsync(id, dto);
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

    [HttpDelete(Endpoints.IntegrationCredential.Delete)]
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
