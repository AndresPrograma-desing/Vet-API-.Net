using System;
using System.Threading.Tasks;
using DTOs;
using Microsoft.AspNetCore.Mvc;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Routes;

namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovimientosInventarioController : ControllerBase
{
    private readonly IMovimientosInventarioService _service;

    public MovimientosInventarioController(IMovimientosInventarioService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<MovimientoInventarioListResponseDTO>> GetMovimientos(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? productId = null,
        [FromQuery] string? movementType = null)
    {
        try
        {
            var result = await _service.GetMovimientosAsync(pageNumber, pageSize, productId, movementType);
            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseErrors.InternalServerError });
        }
    }

    [HttpGet(Endpoints.MovimientosInventarioController.AlertasStockMinimo)]
    public async Task<IActionResult> GetLowStockAlerts()
    {
        try
        {
            var result = await _service.GetLowStockAlertsAsync();
            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseErrors.InternalServerError });
        }
    }
}
