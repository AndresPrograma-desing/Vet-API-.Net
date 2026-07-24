using Microsoft.AspNetCore.Mvc;
using DTOs;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Constants;
using vet_api_Net.Routes;

namespace vet_api_Net.Controller;

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

    [HttpPut(Endpoints.PetsController.Update)]
    public async Task<IActionResult> UpdateMascota([FromRoute] int id, [FromBody] UpdatePetDTO dto)
    {
        try
        {
            var result = await _petsService.UpdateMascotaAsync(id, dto);
            if (result == null) return NotFound(new { message = ResponseMessagesPetsController.MascotaNotFound });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete(Endpoints.PetsController.Delete)]
    public async Task<IActionResult> DeleteMascota([FromRoute] int id)
    {
        try
        {
            var success = await _petsService.DeleteMascotaAsync(id);
            if (!success) return NotFound(new { message = ResponseMessagesPetsController.MascotaNotFound });
            return Ok(new { message = "Mascota eliminada correctamente con todo su historial clínico, citas y consultas asociadas." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
