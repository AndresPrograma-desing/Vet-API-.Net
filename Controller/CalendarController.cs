using System;
using System.Threading.Tasks;
using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Routes;
using static DTOs.CalendarDTO;

//Describe: Controlador de la API de calendario, expone el endpoint para obtener el resumen mensual con los días y slots de agenda de los doctores.
namespace vet_api_Net.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CalendarController : ControllerBase
{
    private readonly ICalendarService _calendarService;

    public CalendarController(ICalendarService calendarService)
    {
        _calendarService = calendarService;
    }

    [HttpGet(Endpoints.Calendar.GetCalendar)]
    public async Task<ActionResult<CalendarResponseDTO>> GetCalendar(
        [FromQuery] int year,
        [FromQuery] int month,
        [FromQuery] int? doctorId = null)
    {
        try
        {
            var calendar = await _calendarService.GetCalendarAsync(year, month, doctorId);
            return Ok(calendar);
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
    [HttpGet(Endpoints.Calendar.CheckAvailability)]
    public async Task<ActionResult<ValidateSlotResponseDTO>> CheckAvailability(
    [FromQuery] int doctorId,
    [FromQuery] DateTime date,
    [FromQuery] string time, 
    [FromQuery] int durationMinutes = 30)
    {
        try
        {
            if (doctorId <= 0)
                return BadRequest(new { error = ResponseMessagesCalendar.RequiredId });

            if (!TimeOnly.TryParse(time, out TimeOnly requestedTime))
                return BadRequest(new { error = ResponseMessagesCalendar.InvalidTimeFormat });

            var result = await _calendarService.CheckDoctorAvailabilityAsync(doctorId, date, requestedTime, durationMinutes);
            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseErrors.InternalServerError });
        }
    }
}
