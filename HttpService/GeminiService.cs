using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using vet_api_Net.Constants;
using vet_api_Net.Infrastructure.Configuration;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Utilities;
using vet_api_Net.Models;

//Describe: Cliente HTTP para generar resúmenes clínicos en lote con Gemini, rotando entre credenciales activas cuando una agota su cuota diaria.
namespace vet_api_Net.HttpServices;

public class GeminiService : IGeminiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IGeminiApiKeySelector _keySelector;
    private readonly GeminiOptions _options;
    private readonly ILogger<GeminiService> _logger;

    public GeminiService(
        IHttpClientFactory httpClientFactory,
        IGeminiApiKeySelector keySelector,
        IOptions<GeminiOptions> options,
        ILogger<GeminiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _keySelector = keySelector;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<Dictionary<int, ResumenClinicoIAResponseDTO>> GenerarResumenesBatchAsync(List<GeminiPatientSummaryRequestDTO> pacientes)
    {
        if (string.IsNullOrEmpty(_options.ApiUrl) || string.IsNullOrEmpty(_options.Model))
        {
            _logger.LogError("Gemini no está configurado (ApiUrl/Model faltante).");
            throw new InvalidOperationException(ResponseMessagesGeminiServices.ServiceUnavailable);
        }

        var requestUrl = _options.ApiUrl.Replace("{model}", _options.Model);
        var prompt = BuildPrompt(pacientes);

        var attemptedKeyIds = new HashSet<int>();

        while (true)
        {
            var credential = await _keySelector.AcquireAvailableKeyAsync();
            if (credential == null || !attemptedKeyIds.Add(credential.Id))
            {
                _logger.LogWarning("Todas las credenciales de Gemini disponibles fueron agotadas o no hay ninguna configurada.");
                throw new InvalidOperationException(ResponseMessagesGeminiServices.AllCredentialsExhausted);
            }

            using var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("x-goog-api-key", credential.SecretValue);

            var requestBody = new
            {
                contents = new[]
                {
                    new { role = "user", parts = new[] { new { text = prompt } } }
                },
                generationConfig = new
                {
                    responseMimeType = "application/json"
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            try
            {
                response = await client.PostAsync(requestUrl, content);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "No se pudo establecer conexión con Gemini en la URL: {ApiUrl}", requestUrl);
                throw new InvalidOperationException(ResponseMessagesGeminiServices.ServiceUnavailable, ex);
            }

            await _keySelector.RegisterUsageAsync(credential.Id);

            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                _logger.LogWarning("Credencial de Gemini {CredentialId} agotó su cuota, se intentará con la siguiente disponible.", credential.Id);
                await _keySelector.MarkExhaustedAsync(credential.Id);
                continue;
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error en respuesta de Gemini: {StatusCode}. Detalles: {Error}", response.StatusCode, errorBody);
                throw new InvalidOperationException(ResponseMessagesGeminiServices.RequestFailed);
            }

            var responseString = await response.Content.ReadAsStringAsync();
            return ParseBatchResponse(responseString);
        }
    }

    private static string BuildPrompt(List<GeminiPatientSummaryRequestDTO> pacientes)
    {
        var jsonOptions = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
        var pacientesJson = JsonSerializer.Serialize(new { pacientes }, jsonOptions);

        return $"{ResponseMessagesGeminiServices.GeminiPrompts.BatchInstruction}\n\n{pacientesJson}";
    }

    private Dictionary<int, ResumenClinicoIAResponseDTO> ParseBatchResponse(string responseString)
    {
        try
        {
            using var doc = JsonDocument.Parse(responseString);
            var text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? string.Empty;

            var deserializeOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = JsonSerializer.Deserialize<List<GeminiPatientSummaryResultDTO>>(text, deserializeOptions) ?? new();

            return results.ToDictionary(r => r.MascotaId, r => new ResumenClinicoIAResponseDTO
            {
                ResumenIa = r.ResumenIa,
                AlertasRiesgoIa = r.AlertasRiesgoIa,
                SugerenciasIa = r.SugerenciasIa
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Respuesta de Gemini con formato inesperado: {Response}", responseString);
            throw new InvalidOperationException(ResponseMessagesGeminiServices.ErrorFormat, ex);
        }
    }
}
