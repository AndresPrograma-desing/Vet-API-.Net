using Microsoft.AspNetCore.Mvc;
using DTOs;
using vet_api_Net.Interfaces.Services;
using vet_api_Net.Constants;
using vet_api_Net.Routes;

namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PetsController : ControllerBase
{
    private readonly IPetsService _petsService;

    public PetsController(IPetsService petsService)
    {
        _petsService = petsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var mascotas = await _petsService.GetAllMascotasAsync();
            return Ok(mascotas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet(Endpoints.PetsController.GetById)]
    public async Task<IActionResult> GetMascota(int id)
    {
        try
        {
            var mascota = await _petsService.GetMascotaByIdAsync(id);
            if (mascota == null) return NotFound(new { message = ResponseMessagesPetsController.MascotaNotFound });
            return Ok(mascota);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
