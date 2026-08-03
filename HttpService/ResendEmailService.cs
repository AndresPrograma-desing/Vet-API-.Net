using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Utilities;

//Describe:
// Este servicio maneja la conexión HTTP directa con el proveedor Resend para enviar correos electrónicos.
// Es un canal de envío reutilizable para cualquier tipo de correo (facturas, citas, consultas, etc.).

namespace vet_api_Net.HttpServices;

public class ResendEmailService : IEmailSenderService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IFailureTrackerUtilities _failureTracker;
    private readonly ISystemConfigRepository _systemConfigRepository;
    private readonly Microsoft.Extensions.DependencyInjection.IServiceScopeFactory _services;

    public ResendEmailService(
        HttpClient httpClient, 
        IConfiguration configuration,
        IFailureTrackerUtilities failureTracker,
        ISystemConfigRepository systemConfigRepository,
        Microsoft.Extensions.DependencyInjection.IServiceScopeFactory services)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _failureTracker = failureTracker;
        _systemConfigRepository = systemConfigRepository;
        _services = services;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string htmlBody, byte[]? attachmentBytes = null, string? attachmentName = null)
    {
        try
        {
            string? apiKey;
            string? fromEmail;
            string? apiUrl;

            using (var scope = _services.CreateScope())
            {
                var systemConfig = await _systemConfigRepository.GetSystemConfigAsync();

                apiKey = systemConfig?.ResendApiKey;
                fromEmail = systemConfig?.ResendFromEmail;
                apiUrl = systemConfig?.ResendApiUrl;
            }

            // Fallback para appsettings.json en desarrollo local
            var appSettingsApiKey = _configuration["ResendSettings:ApiKey"];
            var appSettingsFromEmail = _configuration["ResendSettings:From"];
            var appSettingsApiUrl = _configuration["ResendSettings:ApiUrl"];

            if (string.IsNullOrWhiteSpace(apiKey) || apiKey == "re_8ihXsxrL_NRxgtRcoyqjou3J75MjbJdFo")
            {
                if (!string.IsNullOrWhiteSpace(appSettingsApiKey))
                {
                    apiKey = appSettingsApiKey;
                }
            }

            if (string.IsNullOrWhiteSpace(fromEmail) || fromEmail == "HappyPets <onboarding@resend.dev>")
            {
                if (!string.IsNullOrWhiteSpace(appSettingsFromEmail))
                {
                    fromEmail = appSettingsFromEmail;
                }
            }

            if (string.IsNullOrWhiteSpace(apiUrl) || apiUrl == "https://api.resend.com/emails")
            {
                if (!string.IsNullOrWhiteSpace(appSettingsApiUrl))
                {
                    apiUrl = appSettingsApiUrl;
                }
            }

            if (_failureTracker.IsBlocked("ResendEmail"))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(apiUrl) || 
                string.IsNullOrWhiteSpace(apiKey) || 
                string.IsNullOrWhiteSpace(fromEmail))
            {
                _failureTracker.RecordFailure("ResendEmail");
                return false;
            }

            var requestBody = new Dictionary<string, object>
            {
                { "from", fromEmail! },
                { "to", new[] { to } },
                { "subject", subject },
                { "html", htmlBody }
            };

            if (attachmentBytes != null && !string.IsNullOrEmpty(attachmentName))
            {
                var base64Content = Convert.ToBase64String(attachmentBytes);
                requestBody["attachments"] = new[]
                {
                    new { filename = attachmentName, content = base64Content }
                };
            }

            var jsonPayload = JsonSerializer.Serialize(requestBody);
            using var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            
            request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await _httpClient.SendAsync(request);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                string friendlyMessage = ParseResendError(errorDetails);
                RecordFailureAndThrow(friendlyMessage);
                _failureTracker.RecordFailure("ResendEmail");
                return false;
            }

            _failureTracker.Reset("ResendEmail");
            return true;
        }
        catch (Exception)
        {
            _failureTracker.RecordFailure("ResendEmail");
            return false;
        }
    }

    private void RecordFailureAndThrow(string message, Exception? innerException = null)
    {
        _failureTracker.RecordFailure("ResendEmail");
        if (innerException != null)
        {
            throw new InvalidOperationException(message, innerException);
        }
        throw new InvalidOperationException(message);
    }

    private string ParseResendError(string errorDetails)
    {
        try
        {
            using var doc = JsonDocument.Parse(errorDetails);
            var root = doc.RootElement;
            
            if (root.TryGetProperty("message", out var messageProp))
            {
                string message = messageProp.GetString() ?? "";
                
                if (message.Contains("API key is invalid", StringComparison.OrdinalIgnoreCase))
                {
                    return ResponseMessagesEmailsController.ProviderErrorInvalidKey;
                }
                if (message.Contains("Invalid `to` field", StringComparison.OrdinalIgnoreCase) || 
                    message.Contains("needs to follow the", StringComparison.OrdinalIgnoreCase))
                {
                    return ResponseMessagesEmailsController.ProviderErrorInvalidTo;
                }
                if (message.Contains("restricted", StringComparison.OrdinalIgnoreCase) || 
                    message.Contains("send emails to your own email address", StringComparison.OrdinalIgnoreCase) ||
                    message.Contains("not verified", StringComparison.OrdinalIgnoreCase))
                {
                    return ResponseMessagesEmailsController.ProviderErrorSandboxRestricted;
                }
                if (message.Contains("validation", StringComparison.OrdinalIgnoreCase))
                {
                    return ResponseMessagesEmailsController.ProviderErrorValidation(message);
                }
                
                return ResponseMessagesEmailsController.ProviderErrorGeneric(message);
            }
        }
        catch
        {
            // Si hay un error al parsear el JSON, caer en la excepción genérica
        }

        return $"{ResponseMessagesEmailsController.ProviderError} Detalles: {errorDetails}";
    }
}
