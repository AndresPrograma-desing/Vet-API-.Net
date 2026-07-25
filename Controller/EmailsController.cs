using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Routes;
using DTOs;

namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailsController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly ISystemConfigRepository _systemConfigRepository;

    public EmailsController(IEmailService emailService, ISystemConfigRepository systemConfigRepository)
    {
        _emailService = emailService;
        _systemConfigRepository = systemConfigRepository;
    }

    [HttpPost(Endpoints.EmailsController.DispatchEmail)]
    public async Task<IActionResult> DispatchEmail([FromRoute] int entityId, [FromQuery] int emailTypeCode )
    {
        try
        {
            string typeEmail = EmailTypes.GetEmailTypeString(emailTypeCode);
            if (typeEmail == "Unknown")
            {
                return BadRequest(new { success = false, message = "Tipo de email no válido (Código de correo desconocido)." });
            }

            var result = await _emailService.DispatchEmailAsync(entityId, typeEmail);

            if (result.IsDispatched)
            {
                return Ok(new 
                { 
                    success = true, 
                    message = string.Format(ResponseMessagesEmailsController.DispatchSuccess, result.ClientName),
                    data = result 
                });
            }
 
            return StatusCode(502, new 
            { 
                success = false, 
                message = string.Format(ResponseMessagesEmailsController.DispatchFailure, result.ClientName),
                data = result 
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new 
            { 
                success = false, 
                message = ResponseMessagesEmailsController.DispatchCriticalFailure, 
                details = ex.Message 
            });
        }
    }

    [HttpGet(Endpoints.EmailsController.TestEmail)]
    public async Task<IActionResult> TestEmail([FromQuery] string to)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(to))
            {
                return BadRequest(new { success = false, message = ResponseMessagesEmailsController.MissingToParameter });
            }

            var emailSenderService = HttpContext.RequestServices.GetRequiredService<IEmailSenderService>();
            
            bool sent = await emailSenderService.SendEmailAsync(
                to, 
                ResponseMessagesEmailsController.TestEmailSubject, 
                ResponseMessagesEmailsController.TestEmailBody
            );

            if (sent)
            {
                return Ok(new { success = true, message = string.Format(ResponseMessagesEmailsController.TestEmailSuccess, to) });
            }

            return StatusCode(500, new { success = false, message = ResponseMessagesEmailsController.TestEmailFailure });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ResponseErrors.InternalServerError, details = ex.Message });
        }
    }

    [HttpGet(Endpoints.EmailsController.GetTemplates)]
    public async Task<IActionResult> GetAllTemplates()
    {
        try
        {
            var templates = await _emailService.GetAllEmailTemplatesAsync();
            return Ok(new { success = true, data = templates });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ResponseErrors.InternalServerError, details = ex.Message });
        }
    }

    [HttpGet(Endpoints.EmailsController.GetTemplateById)]
    public async Task<IActionResult> GetTemplateById([FromRoute] int id)
    {
        try
        {
            var template = await _emailService.GetEmailTemplateByIdAsync(id);
            if (template == null)
            {
                return NotFound(new { success = false, message = ResponseMessagesEmailsController.TemplateNotFoundById });
            }
            return Ok(new { success = true, data = template });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ResponseErrors.InternalServerError, details = ex.Message });
        }
    }

    [HttpPut(Endpoints.EmailsController.UpdateTemplate)]
    public async Task<IActionResult> UpdateTemplate([FromRoute] int id, [FromBody] UpdateEmailTemplateDTO dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = ResponseMessagesEmailsController.InvalidPayload, errors = ModelState });
            }

            var updatedTemplate = await _emailService.UpdateEmailTemplateAsync(id, dto);

            if (updatedTemplate == null)
            {
                return NotFound(new { success = false, message = ResponseMessagesEmailsController.TemplateNotFoundById });
            }

            return Ok(new { success = true, message = ResponseMessagesEmailsController.UpdateSuccess, data = updatedTemplate });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ResponseMessagesEmailsController.UpdateFailure, details = ex.Message });
        }
    }
    [HttpGet(Endpoints.EmailsController.DataResend)]
    public async Task<IActionResult> DataResend()
    {
        try
        {
            var systemConfig = await _systemConfigRepository.GetSystemConfigAsync();

            var data = new DataResendDto
            {
                ClientEmail = systemConfig?.ResendFromEmail ?? string.Empty,
                ApiKey = systemConfig?.ResendApiKey ?? string.Empty,
                UrlResend = !string.IsNullOrWhiteSpace(systemConfig?.ResendApiUrl) && Uri.TryCreate(systemConfig.ResendApiUrl, UriKind.Absolute, out var uri) ? uri.GetLeftPart(UriPartial.Authority) : "https://api.resend.com",
                ApiUrl = systemConfig?.ResendApiUrl ?? "https://api.resend.com/emails",
                Active = !string.IsNullOrWhiteSpace(systemConfig?.ResendApiKey) && systemConfig?.ResendApiKey != "re_8ihXsxrL_NRxgtRcoyqjou3J75MjbJdFo"
            };
            return Ok(new { success = true, data = data });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ResponseErrors.InternalServerError, details = ex.Message });
        }
    }

    [HttpPost(Endpoints.EmailsController.UpdateResend)]
    public async Task<IActionResult> UpdateResend([FromBody] DataResendDto dto)
    {
        try
        {
            if (dto == null)
            {
                return BadRequest(new { success = false, message = "Datos no válidos." });
            }

            await _systemConfigRepository.UpdateResendConfigAsync(dto.ApiKey, dto.ClientEmail, dto.ApiUrl);
            return Ok(new { success = true, message = "Configuración de Resend actualizada con éxito." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ResponseErrors.InternalServerError, details = ex.Message });
        }
    }
}
