using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using DTOs;
using vet_api_Net.Constants;
using vet_api_Net.Routes;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Services.Clients;
using vet_api_Net.Interfaze.Utilities;

namespace vet_api_Net.Controllers;

//Describe: Controlador HTTP de autenticación del Portal de Clientes (login passwordless por código de un solo uso enviado por correo) y datos propios del cliente autenticado.
[ApiController]
[Route("api/[controller]")]
public class ClientAuthController : ControllerBase
{
    private readonly IClientAuthService _clientAuthService;
    private readonly IClientService _clientService;
    private readonly ICitasRequestService _citasRequestService;
    private readonly IHistoriaClinicaService _historiaClinicaService;
    private readonly IHistoriaClinicaPdfUtilities _historiaClinicaPdfUtilities;
    private readonly IClientPetService _clientPetService;
    private readonly INotificationService _notificationService;
    private readonly IWebHostEnvironment _env;

    public ClientAuthController(
        IClientAuthService clientAuthService,
        IClientService clientService,
        ICitasRequestService citasRequestService,
        IHistoriaClinicaService historiaClinicaService,
        IHistoriaClinicaPdfUtilities historiaClinicaPdfUtilities,
        IClientPetService clientPetService,
        INotificationService notificationService,
        IWebHostEnvironment env)
    {
        _clientAuthService = clientAuthService;
        _clientService = clientService;
        _citasRequestService = citasRequestService;
        _historiaClinicaService = historiaClinicaService;
        _historiaClinicaPdfUtilities = historiaClinicaPdfUtilities;
        _clientPetService = clientPetService;
        _notificationService = notificationService;
        _env = env;
    }

    private int? GetClientId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(idClaim, out var id) ? id : null;
    }

    [HttpPost(Endpoints.ClientAuth.RequestCode)]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> RequestCode([FromBody] ClientAccessRequestDTO request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Identifier))
        {
            return BadRequest(new { success = false, error = ResponseMessagesClientAuth.RequiredIdentifier });
        }

        try
        {
            var result = await _clientAuthService.RequestLoginCodeAsync(request.Identifier);
            if (!result)
            {
                return NotFound(new { success = false, error = ResponseMessagesClientAuth.ClientNotFound });
            }

            return Ok(new { success = true, message = ResponseMessagesClientAuth.CodeSentSuccess });
        }
        catch (System.Exception)
        {
            return StatusCode(500, new { success = false, error = ResponseErrors.InternalServerError });
        }
    }

    [HttpPost(Endpoints.ClientAuth.VerifyCode)]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> VerifyCode([FromBody] ClientVerifyAccessDTO request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Code))
        {
            return BadRequest(new { error = ResponseMessagesClientAuth.RequiredCode });
        }

        try
        {
            var client = await _clientAuthService.VerifyLoginCodeAsync(request.Code);
            if (client == null)
            {
                return Unauthorized(new { error = ResponseMessagesClientAuth.InvalidCode });
            }

            var token = _clientAuthService.GenerateToken(client);

            return Ok(new ClientLoginResponseDTO
            {
                Token = token,
                Client = new ClientProfileDTO
                {
                    Id = client.Id,
                    Nombre = client.Nombre,
                    Apellido = client.Apellido,
                    Email = client.Email,
                    ImageUrl = client.ImageUrl
                }
            });
        }
        catch (System.Exception)
        {
            return StatusCode(500, new { error = ResponseErrors.InternalServerError });
        }
    }

    [HttpGet(Endpoints.ClientAuth.Me)]
    [Authorize(Roles = Roles.Cliente)]
    public async Task<IActionResult> Me()
    {
        var clientId = GetClientId();
        if (clientId == null) return Unauthorized();

        var client = await _clientService.GetClientAsync(clientId.Value);
        if (client == null) return NotFound(new { error = ResponseMessagesClientAuth.ClientNotFound });

        return Ok(new ClientProfileDTO
        {
            Id = client.Id,
            Nombre = client.Nombre,
            Apellido = client.Apellido,
            Email = client.Email,
            ImageUrl = client.ImageUrl
        });
    }

    [HttpPost(Endpoints.ClientAuth.MeImage)]
    [Authorize(Roles = Roles.Cliente)]
    public async Task<IActionResult> UploadMeImage(IFormFile image)
    {
        var clientId = GetClientId();
        if (clientId == null) return Unauthorized();

        try
        {
            var imageUrl = await _clientPetService.UploadClientImageAsync(clientId.Value, image);
            return Ok(new { image_url = imageUrl });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseMessagesClientPetController.ErrorUploadingImage });
        }
    }

    [HttpPost(Endpoints.ClientAuth.MeMascotaImage)]
    [Authorize(Roles = Roles.Cliente)]
    public async Task<IActionResult> UploadMeMascotaImage([FromRoute] int mascotaId, IFormFile image)
    {
        var clientId = GetClientId();
        if (clientId == null) return Unauthorized();

        var mascotas = await _clientService.GetClientMascotasAsync(clientId.Value);
        if (!mascotas.Exists(m => m.Id == mascotaId))
        {
            return Forbid();
        }

        try
        {
            var imageUrl = await _clientPetService.UploadPetImageAsync(mascotaId, image);
            return Ok(new { image_url = imageUrl });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseMessagesClientPetController.ErrorUploadingImage });
        }
    }

    [HttpGet(Endpoints.ClientAuth.MeNotifications)]
    [Authorize(Roles = Roles.Cliente)]
    public async Task<IActionResult> MeNotifications()
    {
        var clientId = GetClientId();
        if (clientId == null) return Unauthorized();

        var notificaciones = await _notificationService.GetNotificationsByClienteIdAsync(clientId.Value);
        return Ok(notificaciones);
    }

    [HttpPost(Endpoints.ClientAuth.MeNotificationRead)]
    [Authorize(Roles = Roles.Cliente)]
    public async Task<IActionResult> MarkMeNotificationAsRead([FromRoute] int alertId)
    {
        var clientId = GetClientId();
        if (clientId == null) return Unauthorized();

        try
        {
            var success = await _notificationService.MarkClientAlertAsReadAsync(alertId, clientId.Value);
            return Ok(new { success });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseErrors.InternalServerError });
        }
    }

    [HttpGet(Endpoints.ClientAuth.MeMascotas)]
    [Authorize(Roles = Roles.Cliente)]
    public async Task<IActionResult> MeMascotas()
    {
        var clientId = GetClientId();
        if (clientId == null) return Unauthorized();

        var mascotas = await _clientService.GetClientMascotasAsync(clientId.Value);
        return Ok(mascotas);
    }

    [HttpGet(Endpoints.ClientAuth.MeCitas)]
    [Authorize(Roles = Roles.Cliente)]
    public async Task<IActionResult> MeCitas()
    {
        var clientId = GetClientId();
        if (clientId == null) return Unauthorized();

        var citas = await _citasRequestService.GetCitasByClienteAsync(clientId.Value);
        return Ok(citas);
    }

    [HttpGet(Endpoints.ClientAuth.MeHistorial)]
    [Authorize(Roles = Roles.Cliente)]
    public async Task<IActionResult> MeHistorial([FromRoute] int mascotaId)
    {
        var clientId = GetClientId();
        if (clientId == null) return Unauthorized();

        var mascotas = await _clientService.GetClientMascotasAsync(clientId.Value);
        if (!mascotas.Exists(m => m.Id == mascotaId))
        {
            return Forbid();
        }

        var result = await _historiaClinicaService.GetHistoriaClinicaByMascotaIdAsync(mascotaId);
        if (result == null)
        {
            return NotFound(new { message = ResponseMessagesHistoriaClinica.HistoriaClinicaNotFound });
        }

        return Ok(result);
    }

    [HttpGet(Endpoints.ClientAuth.MeHistorialPdf)]
    [Authorize(Roles = Roles.Cliente)]
    public async Task<IActionResult> MeHistorialPdf([FromRoute] int mascotaId)
    {
        var clientId = GetClientId();
        if (clientId == null) return Unauthorized();

        var mascotas = await _clientService.GetClientMascotasAsync(clientId.Value);
        if (!mascotas.Exists(m => m.Id == mascotaId))
        {
            return Forbid();
        }

        try
        {
            var historia = await _historiaClinicaService.GetHistoriaClinicaByMascotaIdAsync(mascotaId);
            if (historia == null)
            {
                return NotFound(new { message = ResponseMessagesHistoriaClinica.HistoriaClinicaNotFound });
            }

            var webRoot = _env?.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var fileBytes = _historiaClinicaPdfUtilities.GenerateHistoriaClinicaPdf(historia, webRoot);
            var fileName = _historiaClinicaPdfUtilities.BuildFileName(historia);

            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseErrors.InternalServerError });
        }
    }
}
