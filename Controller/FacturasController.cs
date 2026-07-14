using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using DTOs;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Services;
using vet_api_Net.Constants;
using vet_api_Net.Routes;

namespace vet_api_Net.Controller;

[ApiController]
[Route("api/[controller]")]
public class FacturasController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;
    private readonly GeneratePdfService _pdfService;
    private readonly IWebHostEnvironment _env;
    private readonly IFacturasRepository _facturasRepository;
    private readonly string? _externalBaseUrl;
    private readonly IMoneyTypeService _moneyTypeService;

    public FacturasController(
        IInvoiceService invoiceService, 
        GeneratePdfService pdfService, 
        IWebHostEnvironment env, 
        IFacturasRepository facturasRepository, 
        IConfiguration config, 
        IMoneyTypeService moneyTypeService)
    {
        _invoiceService = invoiceService;
        _pdfService = pdfService;
        _env = env;
        _facturasRepository = facturasRepository;
        _externalBaseUrl = config["ExternalBaseUrl"];
        _moneyTypeService = moneyTypeService;
    }

    [HttpGet(Endpoints.Facturas.CitaId)]
    public async Task<IActionResult> GetInvoiceForCita(int citaId)
    {
        try
        {
            var dto = await _invoiceService.GenerateInvoiceForCitaAsync(citaId);
            if (dto == null) return NotFound(new { message = ResponseMessagesFacturaController.ConsultaCitaNotFound });

            try
            {
                var webRoot = _env?.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var dir = Path.Combine(webRoot, "facturas");
                if (Directory.Exists(dir) && !string.IsNullOrWhiteSpace(dto.NumeroFactura))
                {
                    var match = Directory.GetFiles(dir, $"*{dto.NumeroFactura}*.pdf").FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(match))
                    {
                        var file = Path.GetFileName(match);
                        dto = dto with { UrlDocx = BuildFileUrl(file) };
                    }
                }
            }
            catch { }

            return Ok(dto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost(Endpoints.Facturas.CitaPdf)]
    public async Task<IActionResult> GeneratePdfForCita(int citaId)
    {
        try
        {
            var dto = await _invoiceService.GenerateInvoiceForCitaAsync(citaId);
            if (dto == null) return NotFound(new { message = ResponseMessagesFacturaController.ConsultaCitaNotFound });

            var consultaId = dto.ConsultaId ?? citaId;
            var webRoot = _env?.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var moneyType = await _moneyTypeService.GetMoneyTypeAsync();
            string currencySymbol = "Bs.";
            if (moneyType != null && moneyType.MoneyName.ToUpper() == "USD")
                currencySymbol = "USD ";
            else if (moneyType != null && moneyType.MoneyName.ToUpper() == "VES")
                currencySymbol = "Bs. ";

            string numeroParaUsar = dto.NumeroFactura;
            if (string.IsNullOrWhiteSpace(numeroParaUsar) || numeroParaUsar.StartsWith("TMP-"))
            {
                numeroParaUsar = $"R-{DateTime.UtcNow:yyyyMMddHHmmss}-{consultaId}";
                dto = dto with { NumeroFactura = numeroParaUsar };
            }

            var fileName = _pdfService.GenerateInvoicePdf(dto, webRoot, currencySymbol);
            var url = BuildFileUrl(fileName);

            try
            {
                await _invoiceService.SaveOrUpdateFacturaPersistenceAsync(citaId, consultaId, numeroParaUsar, url, dto);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = ResponseMessagesFacturaErrors.InvoiceGenerationError });
            }

            return Ok(new { file = fileName, url });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet(Endpoints.Facturas.Files)]
    public IActionResult ListPdfFiles()
    {
        try
        {
            var webRoot = _env?.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var dir = Path.Combine(webRoot, "facturas");
            if (!Directory.Exists(dir)) return Ok(new List<object>());

            var files = Directory.GetFiles(dir, "*.pdf")
                .Select(f =>
                {
                    var info = new FileInfo(f);
                    return new
                    {
                        name = info.Name,
                        url = BuildFileUrl(info.Name),
                        size = info.Length,
                        created = info.CreationTimeUtc
                    };
                })
                .OrderByDescending(x => x.created)
                .ToList();

            return Ok(files);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet(Endpoints.Facturas.Download)]
    public IActionResult DownloadPdf(string fileName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(fileName)) return BadRequest(new { error = ResponseMessagesFacturaController.RequireNameFile });
            var safeName = Path.GetFileName(fileName);
            var webRoot = _env?.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var dir = Path.Combine(webRoot, "facturas");
            var filePath = Path.Combine(dir, safeName);
            if (!System.IO.File.Exists(filePath)) return NotFound(new { error = ResponseMessagesFacturaController.FileNotFound });

            var contentType = "application/pdf";
            return PhysicalFile(filePath, contentType, safeName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPatch(Endpoints.Facturas.StatusPago)]
    public async Task<IActionResult> ChangeEstadoPago(int id, [FromBody] UpdateFacturaEstadoDTO model)
    {
        try
        {
            if (model == null || string.IsNullOrWhiteSpace(model.EstadoPago)) return BadRequest(new { error = ResponseMessagesFacturaController.StatusPagoIsRequired });
            var dto = await _invoiceService.ChangeStatusFacturaAsync(id, model.EstadoPago, model.MetodoPago);
            if (dto == null) return NotFound(new { message = ResponseMessagesFacturaController.FacturaNotFound });
            return Ok(dto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllFacturas()
    {
        try
        {
            var facturas = await _invoiceService.AllFacturasAsync();
            if (facturas == null || facturas.Count == 0)
            {
                return Ok(new { message = ResponseMessagesFacturaController.FacturaNotFound });
            }
            return Ok(facturas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet(Endpoints.Facturas.Settings)]
    public async Task<IActionResult> GetFacturaSettings()
    {
        try
        {
            var settings = await _invoiceService.GetFacturaSettingsAsync();
            return Ok(settings);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    private string BuildFileUrl(string fileName)
    {
        var baseUrl = !string.IsNullOrWhiteSpace(_externalBaseUrl) 
            ? _externalBaseUrl.TrimEnd('/') 
            : $"{Request.Scheme}://{Request.Host}";
            
        return $"{baseUrl}/facturas/{Uri.EscapeDataString(fileName)}";
    }
}