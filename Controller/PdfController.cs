using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Utilities;
using vet_api_Net.Routes;

namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PdfController : ControllerBase
{
    private readonly IConsultasService _consultasService;
    private readonly IConsultaPdfUtilities _consultaPdfUtilities;
    private readonly IUserService _userService;
    private readonly IProfilePdfUtilities _profilePdfUtilities;
    private readonly IMoneyTypeService _moneyTypeService;
    private readonly IWebHostEnvironment _env;
    private readonly IPetVaccinationService _petVaccinationService;
    private readonly IVaccinationCertificatePdfUtilities _vaccinationCertificatePdfUtilities;

    public PdfController(
        IConsultasService consultasService,
        IConsultaPdfUtilities consultaPdfUtilities,
        IUserService userService,
        IProfilePdfUtilities profilePdfUtilities,
        IMoneyTypeService moneyTypeService,
        IWebHostEnvironment env,
        IPetVaccinationService petVaccinationService,
        IVaccinationCertificatePdfUtilities vaccinationCertificatePdfUtilities)
    {
        _consultasService = consultasService;
        _consultaPdfUtilities = consultaPdfUtilities;
        _userService = userService;
        _profilePdfUtilities = profilePdfUtilities;
        _moneyTypeService = moneyTypeService;
        _env = env;
        _petVaccinationService = petVaccinationService;
        _vaccinationCertificatePdfUtilities = vaccinationCertificatePdfUtilities;
    }

    [HttpGet(Endpoints.PdfController.DownloadConsulta)]
    public async Task<IActionResult> DownloadConsultaPdf(int id)
    {
        try
        {
            var consulta = await _consultasService.GetConsultaPdfDataAsync(id);
            if (consulta == null) return NotFound(new { message = ResponseMessagesConsultas.NotFound });

            var moneyType = await _moneyTypeService.GetMoneyTypeAsync();
            string currencySymbol = "Bs.";
            if (moneyType != null && moneyType.MoneyName.ToUpper() == "USD")
                currencySymbol = "USD ";
            else if (moneyType != null && moneyType.MoneyName.ToUpper() == "VES")
                currencySymbol = "Bs. ";

            var webRoot = _env?.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var fileBytes = _consultaPdfUtilities.GenerateConsultaPdf(consulta, webRoot, currencySymbol);
            var fileName = _consultaPdfUtilities.BuildFileName(consulta);

            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseErrors.InternalServerError });
        }
    }

    [HttpGet(Endpoints.PdfController.DownloadCredencial)]
    public async Task<IActionResult> DownloadCredencialPdf(int id)
    {
        try
        {
            var usuario = await _userService.GetCredencialDataAsync(id);
            if (usuario == null) return NotFound(new { message = ResponseMessagesUsers.UserNotFound });

            var webRoot = _env?.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var fileBytes = _profilePdfUtilities.GenerateCredencialPdf(usuario, webRoot);
            var fileName = _profilePdfUtilities.BuildFileName(usuario);

            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseErrors.InternalServerError });
        }
    }

    [HttpGet(Endpoints.PdfController.DownloadCarnet)]
    public async Task<IActionResult> DownloadCarnetPdf(int mascotaId)
    {
        try
        {
            var carnet = await _petVaccinationService.GetCarnetDataAsync(mascotaId);
            if (carnet == null) return NotFound(new { message = ResponseMessagesVaccination.MascotaNotFound });

            var webRoot = _env?.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var fileBytes = _vaccinationCertificatePdfUtilities.GenerateCarnetPdf(carnet, webRoot);
            var fileName = _vaccinationCertificatePdfUtilities.BuildFileName(carnet);

            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseErrors.InternalServerError });
        }
    }
}
