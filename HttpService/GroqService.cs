using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DTOs;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using vet_api_Net.Infrastructure.Configuration;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Constants;

//Describe: Servicio encargado de la comunicación directa y consultas libres a la API de Groq/NowGroq.
namespace vet_api_Net.HttpServices;

public class GroqService : IGroqService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApiSettingsOptions _options;
    private readonly ILogger<GroqService> _logger;
    private readonly IIaConocimientoRepository _iaConocimientoRepository;

    public GroqService(
        IHttpClientFactory httpClientFactory,
        IOptions<ApiSettingsOptions> options,
        ILogger<GroqService> logger,
        IIaConocimientoRepository iaConocimientoRepository)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
        _iaConocimientoRepository = iaConocimientoRepository;
    }

    public async Task<GroqChatResponseDTO> EnviarConsultaAsync(GroqChatRequestDTO dto)
    {
        var apiKey = _options.GroqApiKey;
        var apiUrl = _options.GroqApiUrl;

        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogError("API Key para chat no configurada.");
            throw new InvalidOperationException(ResponseMessagesGroqServices.GroqApiKeyError);
        }

        if (string.IsNullOrEmpty(apiUrl))
        {
            _logger.LogError("API URL para chat no configurada.");
            throw new InvalidOperationException(ResponseMessagesGroqServices.GroqApiError);
        }

        using var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var historyList = new List<object>();
        if (dto.Historial != null)
        {
            foreach (var h in dto.Historial)
            {
                historyList.Add(new
                {
                    rol = h.Role.ToLower() == "assistant" ? "assistant" : "user",
                    mensaje = h.Content
                });
            }
        }

        var requestBody = new
        {
            mensaje = dto.Pregunta,
            historial = historyList.ToArray(),
            contactoId = dto.ContactoId
        };

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody, jsonOptions), Encoding.UTF8, "application/json");

        HttpResponseMessage response;
        try
        {
            response = await client.PostAsync(apiUrl, content);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "No se pudo establecer conexión con el servicio de Groq/NowGroq en la URL: {ApiUrl}", apiUrl);
            throw new InvalidOperationException(ResponseMessagesGroqServices.GroqApiError, ex);
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            var contentType = response.Content.Headers.ContentType?.MediaType;

            _logger.LogError("Error en respuesta de la API de Chat: {StatusCode}. Detalles: {Error}", response.StatusCode, errorContent);

            if ((contentType != null && contentType.Contains("text/html")) || 
                (errorContent != null && errorContent.TrimStart().StartsWith("<")))
            {
                throw new InvalidOperationException(ResponseMessagesGroqServices.GroqApiError);
            }

            throw new InvalidOperationException(ResponseMessagesGroqServices.GroqComunicationError + ": " + response.StatusCode);
        }

        var responseString = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseString);
        var contentResult = doc.RootElement
            .GetProperty("respuesta")
            .GetString();   

        return new GroqChatResponseDTO
        {
            Respuesta = contentResult ?? string.Empty
        };
    }
}
