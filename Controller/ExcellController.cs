using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Services.Clients;
using vet_api_Net.Routes;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Utilities;

namespace vet_api_Net.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExcellController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly ICitasRequestService _citasService;
        private readonly IClientService _clientService;
        private readonly IExcelGeneratorUtilities _excelGenerator;

        public ExcellController(IDashboardService dashboardService, ICitasRequestService citasService, IClientService clientService, IExcelGeneratorUtilities excelGenerator)
        {
            _dashboardService = dashboardService;
            _citasService = citasService;
            _clientService = clientService;
            _excelGenerator = excelGenerator;
        }

        [HttpGet(Endpoints.ExcellController.DownloadDashboard)]
        public async Task<IActionResult> DownloadDashboardExcel([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] bool useUsd = false)
        {
            try
            {
                var stats = await _dashboardService.GetDashboardStatsAsync(startDate, endDate, useUsd);
                if (stats == null) return NotFound(new { message = ResponseErrors.NotFound });

                var fileBytes = _excelGenerator.GenerateDashboardExcel(stats);
                
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Dashboard_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = ResponseErrors.InternalServerError });
            }
        }

        [HttpGet(Endpoints.ExcellController.DownloadCitas)]
        public async Task<IActionResult> DownloadCitasExcel()
        {
            try
            {
                var (citas, _) = await _citasService.GetAllCitasRequestsAsync(pageSize: int.MaxValue);
                if (citas == null) return NotFound(new { message = ResponseErrors.NotFound });

                var fileBytes = _excelGenerator.GenerateCitasExcel(citas);
                
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Citas_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = ResponseErrors.InternalServerError });
            }
        }

        [HttpGet(Endpoints.ExcellController.DownloadClientes)]
        public async Task<IActionResult> DownloadClientesExcel()
        {
            try
            {
                var clientes = await _clientService.GetAllClientsWithDetailsAsync();
                if (clientes.Items == null) return NotFound(new { message = ResponseErrors.NotFound });

                var fileBytes = _excelGenerator.GenerateClientesExcel(clientes.Items);
                
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Clientes_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = ResponseErrors.InternalServerError });
            }
        }
    }
}
