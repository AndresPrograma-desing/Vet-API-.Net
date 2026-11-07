using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DTOs;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Routes;

//Describe: Endpoints para consultar y editar el horario laboral global usado al validar citas.
namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkingHoursController(IWorkingHoursService workingHoursService) : ControllerBase
{
    [Authorize]
    [HttpGet(Endpoints.WorkingHours.Get)]
    public async Task<IActionResult> Get()
    {
        try
        {
            var data = await workingHoursService.GetAsync();
            return Ok(new { success = true, data });
        }
        catch (Exception)
        {
            return StatusCode(500, new { success = false, message = ResponseErrors.InternalServerError });
        }
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut(Endpoints.WorkingHours.Update)]
    public async Task<IActionResult> Update([FromBody] WorkingHoursDTO dto)
    {
        try
        {
            var data = await workingHoursService.UpdateAsync(dto);
            return Ok(new { success = true, message = ResponseMessagesWorkingHours.Updated, data });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { success = false, message = ResponseErrors.InternalServerError });
        }
    }
}
