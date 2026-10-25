using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using DTOs;
using vet_api_Net.Constants;
using vet_api_Net.Infrastructure.Configuration;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Utilities;

//Describe:
// Este servicio maneja la conexión HTTP directa con el proveedor ContextHub para enviar correos electrónicos.
// Reemplaza a ResendEmailService como canal de envío reutilizable para cualquier tipo de correo (facturas, citas, consultas, etc.).
// Reutiliza las columnas resend_api_key/resend_api_url de SystemConfig (BD) como x-api-key/BaseUrl de ContextHub,
// igual que ResendEmailService las leía para Resend, con appsettings (ContextHubOptions) solo como fallback.

namespace vet_api_Net.HttpServices;

public class ContextHubEmailService : IEmailSenderService
{
    private readonly HttpClient _httpClient;
    private readonly ContextHubOptions _options;
    private readonly ApiSettingsOptions _apiSettings;
    private readonly IFailureTrackerUtilities _failureTracker;
    private readonly ISystemConfigRepository _systemConfigRepository;
    private readonly IServiceScopeFactory _services;

    public ContextHubEmailService(
        HttpClient httpClient,
        IOptions<ContextHubOptions> options,
        IOptions<ApiSettingsOptions> apiSettingsOptions,
        IFailureTrackerUtilities failureTracker,
        ISystemConfigRepository systemConfigRepository,
        IServiceScopeFactory services)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _apiSettings = apiSettingsOptions.Value;
        _failureTracker = failureTracker;
        _systemConfigRepository = systemConfigRepository;
        _services = services;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string htmlBody, byte[]? attachmentBytes = null, string? attachmentName = null)
    {
        try
        {
            string? mailsApiKey;
            string? baseUrl;

            using (var scope = _services.CreateScope())
            {
                var systemConfig = await _systemConfigRepository.GetSystemConfigAsync();

                mailsApiKey = systemConfig?.EmailProviderApiKey;
                baseUrl = systemConfig?.EmailProviderApiUrl;
            }

            // Fallback para appsettings.json en desarrollo local
            if (string.IsNullOrWhiteSpace(mailsApiKey))
            {
                mailsApiKey = _options.MailsApiKey;
            }

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                baseUrl = _options.BaseUrl;
            }

            if (_failureTracker.IsBlocked("ContextHubEmail"))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(mailsApiKey))
            {
                _failureTracker.RecordFailure("ContextHubEmail");
                return false;
            }

            var payload = new ContextHubSendMailRequest
            {
                To = to,
                Subject = subject,
                Html = htmlBody,
                FromName = _apiSettings.SystemName
            };

            if (attachmentBytes != null && !string.IsNullOrEmpty(attachmentName))
            {
                payload.Attachments = new List<ContextHubMailAttachment>
                {
                    new()
                    {
                        Filename = attachmentName,
                        ContentType = "application/octet-stream",
                        ContentBase64 = Convert.ToBase64String(attachmentBytes)
                    }
                };
            }

            var url = $"{baseUrl.TrimEnd('/')}/api/v1/mails/send";
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            if (!string.IsNullOrWhiteSpace(_options.AnonKey))
            {
                request.Headers.Add("apikey", _options.AnonKey);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.AnonKey);
            }
            request.Headers.Add("x-api-key", mailsApiKey);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                string friendlyMessage = ParseContextHubError(response.StatusCode, errorBody);
                _failureTracker.RecordFailure("ContextHubEmail");
                throw new InvalidOperationException(friendlyMessage);
            }

            _failureTracker.Reset("ContextHubEmail");
            return true;
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception)
        {
            _failureTracker.RecordFailure("ContextHubEmail");
            return false;
        }
    }

    private string ParseContextHubError(System.Net.HttpStatusCode statusCode, string errorBody)
    {
        try
        {
            var error = JsonSerializer.Deserialize<ContextHubMailErrorResponseDTO>(errorBody)?.Error;
            if (error != null)
            {
                if (statusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return ResponseMessagesEmailsController.ContextHubUnauthorized;
                }
                if (statusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    return ResponseMessagesEmailsController.ContextHubRateLimited;
                }
                if (error.Type == "validation")
                {
                    return ResponseMessagesEmailsController.ContextHubValidationError(error.Message ?? string.Empty);
                }
                if (error.Type == "provider_error")
                {
                    return ResponseMessagesEmailsController.ContextHubProviderError;
                }

                return ResponseMessagesEmailsController.ContextHubGenericError(error.Message ?? string.Empty);
            }
        }
        catch
        {
            // Si hay un error al parsear el JSON, caer en la excepción genérica
        }

        return $"{ResponseMessagesEmailsController.ContextHubProviderError} Detalles: {errorBody}";
    }
}
