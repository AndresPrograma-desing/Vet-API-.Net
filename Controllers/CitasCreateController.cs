using Microsoft.AspNetCore.Mvc;
using DTOs;
using vet_api_Net.Interfaces.Services;
using System;
using vet_api_Net.Constants;
using vet_api_Net.Routes;

namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/citas")]
public class CitasCreateController : ControllerBase
{
    private readonly ICreateCitaService _createCitaService;

    public CitasCreateController(ICreateCitaService createCitaService)
    {
        _createCitaService = createCitaService;
    }

    [HttpPost(Endpoints.Citas.Create)]
    public async Task<IActionResult> CreateCita([FromBody] CreateCitaDTO dto)
    {
        try
        {
            var cita = await _createCitaService.CreateCitaAsync(dto);
            var result = new
            {
                id = cita.Id,
                mascota_id = cita.MascotaId,
                doctor_id = cita.DoctorId,
                secretaria_id = cita.SecretariaId,
                fecha_cita = cita.FechaCita,
                hora_cita = cita.HoraCita.ToString("HH:mm:ss"),
                motivo = cita.Motivo,
                tipo_cita = cita.TipoCita,
                estado = cita.Estado,
                notas = cita.Notas,
                metodo_pago_id = cita.MetodoPagoId,
                metodo_pago = cita.MetodoPago != null ? cita.MetodoPago.Nombre : null
            };
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseMessagesCitas.ErrorInterno });
        }
    }
}
