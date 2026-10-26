using System;
using System.Threading.Tasks;
using DTOs;
using Microsoft.AspNetCore.Mvc;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Services;

namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HistorialPrecioController : ControllerBase
{
    private readonly IHistorialPrecioService _service;

    public HistorialPrecioController(IHistorialPrecioService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<HistorialPrecioListResponseDTO>> GetHistorial(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? productId = null)
    {
        try
        {
            var result = await _service.GetHistorialAsync(pageNumber, pageSize, productId);
            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseErrors.InternalServerError });
        }
    }
}
