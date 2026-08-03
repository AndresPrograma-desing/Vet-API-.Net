using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using DTOs;
using vet_api_Net.Constants;
using vet_api_Net.Infrastructure.Configuration;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Services.Clients;
using vet_api_Net.Repositories;

// Describe:
// Servicio encargado de procesar la lógica de negocio para el envío de correos electrónicos y la gestión de la configuración del proveedor (Resend).

namespace vet_api_Net.Services;

public class EmailService : IEmailService
{
    private readonly IInvoiceExternalRepository _repository;
    private readonly IEmailSenderService _emailSenderService;
    private readonly ApiSettingsOptions _apiSettings;
    private readonly TemplatesHTML _templatesHTML;
    private readonly IWebHostEnvironment _env;
    private readonly IEmailTemplateRepository _templateRepository;
    private readonly IAuthService _authService;
    private readonly IClientService _clientService;
    private readonly ISystemConfigRepository _systemConfigRepository;

    public EmailService(
        IInvoiceExternalRepository repository,
        IEmailSenderService emailSenderService,
        IOptions<ApiSettingsOptions> apiSettingsOptions,
        IOptions<TemplatesHTML> templatesHTML,
        IWebHostEnvironment env,
        IEmailTemplateRepository templateRepository,
        IAuthService authService,
        IClientService clientService,
        ISystemConfigRepository systemConfigRepository)
    {
        _repository = repository;
        _emailSenderService = emailSenderService;
        _apiSettings = apiSettingsOptions.Value;
        _templatesHTML = templatesHTML.Value;
        _env = env;
        _templateRepository = templateRepository;
        _authService = authService;
        _clientService = clientService;
        _systemConfigRepository = systemConfigRepository;
    }

    public async Task<InvoiceDispatchResponseDTO> DispatchEmailAsync(int entityId, string typeEmail)
    {
        string clientEmail = "";
        string clientFullName = "";
        string petName = ResponseMessagesWSMessageAPI.PetsDefault;
        string facturaNumero = ResponseMessagesFactura.NA;
        int facturaId = 0;
        byte[]? pdfBytes = null;
        string? pdfName = null;

        try
        { 
            var emailTemplate = await _templateRepository.GetTemplateByTypeAsync(typeEmail);
            if (emailTemplate == null)
            {
                throw new InvalidOperationException(ResponseMessagesEmailsController.TemplateNotFound);
            } 

            if (typeEmail.Equals(_templatesHTML.Recibos, StringComparison.OrdinalIgnoreCase) || 
                typeEmail.Equals("Citas", StringComparison.OrdinalIgnoreCase))
            { 
                var cita = await _repository.GetCitaWithDetailsAsync(entityId)
                    ?? throw new KeyNotFoundException(ResponseMessagesWSMessageAPI.CitaNotFound);

                var consulta = await _repository.GetConsultaByCitaOrPetAsync(cita.Id, cita.MascotaId);

                var cliente = consulta?.Mascota?.Cliente ?? cita.Mascota?.Cliente;
                if (cliente == null || string.IsNullOrWhiteSpace(cliente.Email))
                {
                    throw new InvalidOperationException(ResponseMessagesEmailsController.ClientNoEmail);
                }

                clientEmail = cliente.Email;
                clientFullName = $"{cliente.Nombre} {cliente.Apellido}".Trim();
                petName = consulta?.Mascota?.Nombre ?? cita.Mascota?.Nombre ?? petName;

                if (typeEmail.Equals(_templatesHTML.Recibos, StringComparison.OrdinalIgnoreCase))
                {
                    if (cita.Estado != Status.Completed)
                        throw new InvalidOperationException(ResponseMessagesFacturaErrors.OnlyCitaAllowed);

                    if (consulta == null) 
                        throw new KeyNotFoundException(ResponseMessagesWSMessageAPI.ConsultaNotFound);

                    var factura = await _repository.GetFacturaByConsultaIdAsync(consulta.Id)
                        ?? throw new KeyNotFoundException(ResponseMessagesWSMessageAPI.FacturaNotFound);
                    
                    facturaNumero = factura.NumeroFactura;
                    facturaId = factura.Id;

                    var (bytes, name) = await LoadInvoicePdfAsync(factura.UrlDocx, factura.NumeroFactura);
                    pdfBytes = bytes;
                    pdfName = name;
                    
                    if (pdfBytes == null)
                        throw new InvalidOperationException(string.Format(ResponseMessagesEmailsController.PdfNotFound, factura.NumeroFactura));
                }
            }
            else if (typeEmail.Equals("BienvenidaUsuario", StringComparison.OrdinalIgnoreCase) || 
                     typeEmail.Equals(_templatesHTML.ResendPassword, StringComparison.OrdinalIgnoreCase))
            { 
                var user = await _authService.GetUserByIdAsync(entityId)
                    ?? throw new KeyNotFoundException(ResponseMessagesUsers.UserNotFound);
                
                if (string.IsNullOrWhiteSpace(user.Email))
                    throw new InvalidOperationException(ResponseMessagesUsers.EmailNotConfigured);

                clientEmail = user.Email;
                clientFullName = $"{user.Nombre} {user.Apellido}".Trim();
            }
            else 
            { 
                var cliente = await _clientService.GetClientAsync(entityId)
                    ?? throw new KeyNotFoundException(ResponseMessagesClient.ClientNotFound);

                if (string.IsNullOrWhiteSpace(cliente.Email))
                    throw new InvalidOperationException(ResponseMessagesEmailsController.ClientNoEmail);

                clientEmail = cliente.Email;
                clientFullName = $"{cliente.Nombre} {cliente.Apellido}".Trim();
            }

            string htmlBody = emailTemplate.HtmlCode; 

            htmlBody = htmlBody.Replace("{{client_name}}", clientFullName);
            htmlBody = htmlBody.Replace("{{system_name}}", _apiSettings.SystemName);
            htmlBody = htmlBody.Replace("{{pet_name}}", petName);

            string subject = $"{typeEmail} - {_apiSettings.SystemName}";

            if (typeEmail.Equals(_templatesHTML.Recibos, StringComparison.OrdinalIgnoreCase))
            {
                subject = $"Recibo {facturaNumero} - {_apiSettings.SystemName}";
                htmlBody = htmlBody.Replace("{{factura_numero}}", facturaNumero);
                htmlBody = htmlBody.Replace("{{fecha_emision}}", DateTime.Now.ToString("dd/MM/yyyy"));
            }
            else if (typeEmail.Equals(_templatesHTML.ResetPassword, StringComparison.OrdinalIgnoreCase))
            {
                subject = $"Recuperar Contraseña - {_apiSettings.SystemName}";

                string dummyLink = "https://tu-veterinaria.com/reset-password?token=dummy_token_123456abc";
                string dummyCode = "123-456";
                htmlBody = htmlBody.Replace("{{recovery_link}}", dummyLink);
                htmlBody = htmlBody.Replace("{{recovery_code}}", dummyCode);
            }
            
            bool emailResult = await _emailSenderService.SendEmailAsync(clientEmail, subject, htmlBody, pdfBytes, pdfName);

            return new InvoiceDispatchResponseDTO
            {
                InvoiceId = facturaId,
                InvoiceNumber = facturaNumero,
                IsDispatched = emailResult,
                ClientName = clientFullName,
                DestinationPhone = ResponseMessagesFactura.NA
            };
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IEnumerable<EmailsResponsesDTO>> GetAllEmailTemplatesAsync()
    {
        var templates = await _templateRepository.GetAllTemplatesAsync();
        return templates.Select(t => new EmailsResponsesDTO
        {
            Id = t.Id,
            HtmlCode = t.HtmlCode,
            TypeEmail = t.TypeEmail,
            Update = t.Update
        });
    }

    public async Task<EmailsResponsesDTO?> GetEmailTemplateByIdAsync(int id)
    {
        var t = await _templateRepository.GetTemplateByIdAsync(id);
        if (t == null) return null;
        
        return new EmailsResponsesDTO
        {
            Id = t.Id,
            HtmlCode = t.HtmlCode,
            TypeEmail = t.TypeEmail,
            Update = t.Update
        };
    }

    public async Task<EmailsResponsesDTO?> UpdateEmailTemplateAsync(int id, UpdateEmailTemplateDTO dto)
    {
        var template = await _templateRepository.GetTemplateByIdAsync(id);
        if (template == null) return null;

        template.HtmlCode = dto.HtmlCode;
        template.TypeEmail = dto.TypeEmail;
        template.Update = DateTime.Now;

        bool success = await _templateRepository.UpdateTemplateAsync(template);
        
        if (!success) return null;

        return new EmailsResponsesDTO
        {
            Id = template.Id,
            HtmlCode = template.HtmlCode,
            TypeEmail = template.TypeEmail,
            Update = template.Update
        };
    }

    public async Task<DataResendDto> GetResendConfigAsync()
    {
        var systemConfig = await _systemConfigRepository.GetSystemConfigAsync();

        bool isApiKeyConfigured = !string.IsNullOrWhiteSpace(systemConfig?.ResendApiKey);

        return new DataResendDto
        {
            ClientEmail = systemConfig?.ResendFromEmail ?? string.Empty,
            ApiKey = string.Empty, // Protegemos la ApiKey para no exponerla al cliente
            UrlResend = !string.IsNullOrWhiteSpace(systemConfig?.ResendApiUrl) && Uri.TryCreate(systemConfig.ResendApiUrl, UriKind.Absolute, out var uri) 
                ? uri.GetLeftPart(UriPartial.Authority) 
                : "https://api.resend.com",
            ApiUrl = systemConfig?.ResendApiUrl ?? "https://api.resend.com/emails",
            Active = isApiKeyConfigured
        };
    }

    public async Task UpdateResendConfigAsync(DataResendDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto), "Los datos no son válidos.");
        }

        var currentConfig = await _systemConfigRepository.GetSystemConfigAsync();
        
        string apiKeyToSave = string.IsNullOrWhiteSpace(dto.ApiKey)
            ? currentConfig?.ResendApiKey ?? string.Empty
            : dto.ApiKey;

        await _systemConfigRepository.UpdateResendConfigAsync(apiKeyToSave, dto.ClientEmail, dto.ApiUrl);
    }

    private async Task<(byte[]? bytes, string? name)> LoadInvoicePdfAsync(string? urlDocx, string numeroFactura)
    {
        var webRoot = _env?.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var dir = Path.Combine(webRoot, "facturas");

        if (!Directory.Exists(dir)) return (null, null);

        if (!string.IsNullOrWhiteSpace(urlDocx))
        {
            string? fileName = null;
            try
            {
                var uri = new Uri(urlDocx);
                fileName = Path.GetFileName(uri.LocalPath);
            }
            catch
            {
                fileName = urlDocx.Split('/').LastOrDefault();
            }

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                var filePath = Path.Combine(dir, fileName);
                if (File.Exists(filePath))
                {
                    var bytes = await File.ReadAllBytesAsync(filePath);
                    return (bytes, fileName);
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(numeroFactura))
        {
            try
            {
                var match = Directory.GetFiles(dir, $"*{numeroFactura}*.pdf").FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(match) && File.Exists(match))
                {
                    var bytes = await File.ReadAllBytesAsync(match);
                    var name = Path.GetFileName(match);
                    return (bytes, name);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        return (null, null);
    }
}