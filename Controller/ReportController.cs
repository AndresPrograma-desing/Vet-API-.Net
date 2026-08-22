using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.IO;
using DTOs;
using vet_api_Net.Models;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Services;
using Microsoft.Extensions.Configuration;
using vet_api_Net.ReportSettings;
using vet_api_Net.Data;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Constants;
using vet_api_Net.Routes;

namespace vet_api_Net.Controller
{
	[ApiController]
	[Route("api/[controller]")]
	public class ReportController : ControllerBase
	{
		private readonly IReportSystemService _reportService;
        private readonly GenerateReportExcel _excelGen;
        private readonly IWebHostEnvironment _env;
        private readonly string? _externalBaseUrl;
		private readonly AppDbContext _context;


		public ReportController(IReportSystemService reportService, GenerateReportExcel excelGen, IWebHostEnvironment env, IConfiguration config, AppDbContext context)
		{
			_reportService = reportService;
			_excelGen = excelGen;
			_env = env;
			_externalBaseUrl = config["ExternalBaseUrl"];
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<object>>> GetAll()
		{
			var reportes = await _reportService.GetAllAsync();
			var quantity = reportes.Count();
			var baseUrl = !string.IsNullOrWhiteSpace(_externalBaseUrl)
				? _externalBaseUrl!.TrimEnd('/')
				: $"{Request.Scheme}://{Request.Host}";
			var result = reportes.Select(r => new {
				id = r.Id,
				fecha = r.FechaCreacion,
				url = $"{baseUrl}/api/report/excel/{r.Id}",
				quantity = quantity
			});
			return Ok(result);
		}
		

		[HttpGet(Endpoints.ReportController.GetById)]
		public async Task<ActionResult<Reporte>> GetById(int id)
		{
			var reporte = await _reportService.GetByIdAsync(id);
			if (reporte == null)
			{
				return NotFound();
			}

			return Ok(reporte);
		}

		[HttpPost]
		public async Task<ActionResult<Reporte>> Create([FromBody] Reporte reporte)
		{
			var created = await _reportService.CreateAsync(reporte);
			return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
		}

		[HttpDelete(Endpoints.ReportController.Delete)]
		public async Task<IActionResult> Delete(int id)
		{
			var deleted = await _reportService.DeleteAsync(id);
			if (!deleted)
			{
				return NotFound();
			}

			return NoContent();
		}

		[HttpPost(Endpoints.ReportController.GenerateFull)]
		public async Task<ActionResult<Reporte>> GenerateFullReport([FromQuery] string generadoPor)
		{
			var reporte = await _reportService.GenerateFullSystemReportAsync(generadoPor);
			return CreatedAtAction(nameof(GetById), new { id = reporte.Id }, reporte);
		}

		[HttpGet(Endpoints.ReportController.GenerateFullReport)]
		public async Task<ActionResult<object>> GetFullReportData(int id)
		{
			var reporte = await _reportService.GetByIdAsync(id);
			if (reporte == null)
			{
				return NotFound();
			}

			var data = System.Text.Json.JsonSerializer.Deserialize<object>(reporte.Datos ?? "{}");
			return Ok(data);
		}

		[HttpPost(Endpoints.ReportController.CreateExcel)]
		public async Task<IActionResult> CreateExcel(int id)
		{
			try
			{
				var reporte = await _reportService.GetByIdAsync(id);
				if (reporte == null) return NotFound();

				var excelBytes = _excelGen.GenerateExcelFromReport(reporte);

				var webRoot = _env?.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
				var dir = Path.Combine(webRoot, "Reports");
				Directory.CreateDirectory(dir);

				var fileName = $"Reporte_{id}_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
				var fullPath = Path.Combine(dir, fileName);
				await System.IO.File.WriteAllBytesAsync(fullPath, excelBytes);

				var baseUrl = !string.IsNullOrWhiteSpace(_externalBaseUrl)
					? _externalBaseUrl!.TrimEnd('/')
					: $"{Request.Scheme}://{Request.Host}";

				var url = $"{baseUrl}/Reports/{Uri.EscapeDataString(fileName)}";
				return Ok(new { file = fileName, url });
			}
			catch (Exception)
			{
				return StatusCode(500, new { error = ResponseErrors.InternalServerError });
			}
		}

		[HttpGet(Endpoints.ReportController.GetExcel)]
		public async Task<IActionResult> DownloadExcel(int id)
		{
			var reporte = await _reportService.GetByIdAsync(id);
			if (reporte == null) return NotFound();

			var excelBytes = _excelGen.GenerateExcelFromReport(reporte);
			return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Reporte_{id}.xlsx");
		}
[HttpPost(Endpoints.ReportController.UpdateRetentionDays)]
public async Task<IActionResult> UpdateRetentionDays([FromBody] UpdateReportRetentionDaysDTO data)
{
    if (data.Days <= 0)
        return BadRequest(new { message = ResponseMessagesReportController.MinNumber });

    var setting = await _context.WorkerConfigs.FirstOrDefaultAsync(w => w.WorkerName == WorkerNames.DeleteReportWorker);

    if (setting == null)
    {
        setting = new WorkerConfig { WorkerName = WorkerNames.DeleteReportWorker, RetentionValue = data.Days };
        _context.WorkerConfigs.Add(setting);
    }
    else
    {
        setting.RetentionValue = data.Days;
        setting.LastUpdated = DateTime.UtcNow;
    }

    await _context.SaveChangesAsync();

    return Ok(new {
        message = ResponseMessagesReportController.RetentionDaysUpdated(data.Days)
    });
}

[HttpPost(Endpoints.ReportController.ToggleAutoDelete)]
public async Task<IActionResult> ToggleAutoDelete([FromBody] bool enable)
{
    var setting = await _context.WorkerConfigs.FirstOrDefaultAsync(w => w.WorkerName == WorkerNames.DeleteReportWorker);

    if (setting == null)
    {
        setting = new WorkerConfig { WorkerName = WorkerNames.DeleteReportWorker, IsEnabled = enable, RetentionValue = 30 };
        _context.WorkerConfigs.Add(setting);
    }
    else
    {
        setting.IsEnabled = enable;
        setting.LastUpdated = DateTime.UtcNow;
    }

    await _context.SaveChangesAsync();

    string status = enable ? ResponseMessagesReportController.Enabled : ResponseMessagesReportController.Disabled;
    return Ok(new { message = $"{ResponseMessagesReportController.AutoDeleteStatus}{status}." });
}
[HttpPost(Endpoints.ReportController.ToggleAutoGenerate)]
public async Task<IActionResult> ToggleAutoGenerate([FromBody] bool enable)
{
	var setting = await _context.WorkerConfigs.FirstOrDefaultAsync(w => w.WorkerName == WorkerNames.AutoGenerateReportWorker);

	if (setting == null)
	{
		setting = new WorkerConfig { WorkerName = WorkerNames.AutoGenerateReportWorker, GenerateEnabled = enable };
		_context.WorkerConfigs.Add(setting);
	}
	else
	{
		setting.GenerateEnabled = enable;
		setting.LastUpdated = DateTime.UtcNow;
	}

	await _context.SaveChangesAsync();

	string status = enable ? ResponseMessagesReportController.Enabled : ResponseMessagesReportController.Disabled;
	return Ok(new { message = $"{ResponseMessagesReportController.AutoGenerateStatus}{status}." });
}

[HttpGet(Endpoints.ReportController.Settings)]
 public async Task<ActionResult> GetReportSettings()
		{
			var setting = await _reportService.IsEnabledAsync();
			
			return Ok(setting);
		}
}
}