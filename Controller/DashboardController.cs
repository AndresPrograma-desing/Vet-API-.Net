using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Routes;

namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet(Endpoints.DashboardController.Stats)]
    public async Task<IActionResult> GetStats([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null, [FromQuery] bool useUsd = false)
    {
        try
        {
            var stats = await _dashboardService.GetDashboardStatsAsync(startDate, endDate, useUsd);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ResponseMessagesDashboard.ErrorRetrievingStats, error = ex.Message });
        }
    }
}
