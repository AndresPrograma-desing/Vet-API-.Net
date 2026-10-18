using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using DTOs;
using vet_api_Net.Routes;

using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Utilities;
using vet_api_Net.Constants;

namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConsultasController : ControllerBase
{
    private readonly IConsultasService _consultasService;
    private readonly IConsultaPdfUtilities _pdfUtilities;
    private readonly IMoneyTypeService _moneyTypeService;
    private readonly IWebHostEnvironment _env;
    private readonly string? _externalBaseUrl;

    public ConsultasController(
        IConsultasService consultasService,
        IConsultaPdfUtilities pdfUtilities,
        IMoneyTypeService moneyTypeService,
        IWebHostEnvironment env,
        IConfiguration config)
    {
        _consultasService = consultasService;
        _pdfUtilities = pdfUtilities;
        _moneyTypeService = moneyTypeService;
        _env = env;
        _externalBaseUrl = config["ExternalBaseUrl"];
    }

    [HttpPost(Endpoints.Consultas.Create)]
 [HttpPost]
public async Task<ActionResult<ConsultaRequestDTO>> Create([FromBody] CreateConsultaDTO dto)
{
    try
    {
        var result = await _consultasService.CreateConsultaAsync(dto);
        return StatusCode(201, result);
    }
    catch (ArgumentNullException ex)
    {
        return BadRequest(new { message = ex.Message });
    }
    catch (ArgumentException ex)  
    {
        return BadRequest(new { message = ex.Message });
    }
    catch (KeyNotFoundException ex)
    {
        return NotFound(new { message = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(new { message = ex.Message });
    }
    catch (Exception)
    {
        return StatusCode(500, new { message = ResponseErrors.InternalServerError });
    }
}

    [HttpGet(Endpoints.Consultas.GetById)]
    public async Task<ActionResult<ConsultaRequestDTO>> GetById([FromRoute] int id)
    {
        try
        {
            var result = await _consultasService.GetConsultaByIdAsync(id);
            if (result == null) return NotFound();

            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = ResponseErrors.InternalServerError });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ConsultaRequestDTO>>> GetAll(
        [FromQuery] int? doctorId = null,
        [FromQuery] int? secretariaId = null)
    {
        try
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            int? currentUserId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var parsedId) ? parsedId : null;

            var result = await _consultasService.GetConsultasAsync(role, currentUserId, doctorId, secretariaId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = ResponseErrors.InternalServerError });
        }
    }

    [HttpPatch(Endpoints.Consultas.UpdateReceta)]
    public async Task<ActionResult<ConsultaRequestDTO>> UpdateReceta([FromRoute] int id, [FromBody] UpdateConsultaRecetaDTO dto)
    {
        try
        {
            var result = await _consultasService.UpdateRecetaAsync(id, dto);
            return Ok(result);
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = ResponseMessagesConsultas.ErrorCreated });
        }
    }

    [HttpPost(Endpoints.Consultas.GeneratePdf)]
    public async Task<IActionResult> GeneratePdf([FromRoute] int id)
    {
        try
        {
            var pdfData = await _consultasService.GetConsultaPdfDataAsync(id);
            if (pdfData == null) return NotFound(new { message = ResponseMessagesConsultas.PdfDataNotFound });

            var webRoot = _env?.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var moneyType = await _moneyTypeService.GetMoneyTypeAsync();
            string currencySymbol = "Bs.";
            if (moneyType != null && moneyType.MoneyName.ToUpper() == "USD")
                currencySymbol = "USD ";
            else if (moneyType != null && moneyType.MoneyName.ToUpper() == "VES")
                currencySymbol = "Bs. ";

            var pdfBytes = _pdfUtilities.GenerateConsultaPdf(pdfData, webRoot, currencySymbol);
            var fileName = _pdfUtilities.BuildFileName(pdfData);

            var dir = Path.Combine(webRoot, "consultas");
            Directory.CreateDirectory(dir);
            var filePath = Path.Combine(dir, fileName);
            await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes);

            var url = BuildFileUrl(fileName);

            try
            {
                await _consultasService.UpdateUrlDocxAsync(id, url);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = ResponseMessagesConsultas.PdfPersistenceError });
            }

            return Ok(new { file = fileName, url });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseMessagesConsultas.PdfGenerationError });
        }
    }

    private string BuildFileUrl(string fileName)
    {
        var baseUrl = !string.IsNullOrWhiteSpace(_externalBaseUrl)
            ? _externalBaseUrl.TrimEnd('/')
            : $"{Request.Scheme}://{Request.Host}";

        return $"{baseUrl}/consultas/{Uri.EscapeDataString(fileName)}";
    }
}