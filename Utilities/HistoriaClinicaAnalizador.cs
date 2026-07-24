using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using vet_api_Net.Models;
using DTOs;
using vet_api_Net.Interfaze.Utilities;
using vet_api_Net.Infrastructure.Configuration;
using vet_api_Net.Constants;

//Describe: Analizador clínico experto que delega el procesamiento de historiales médicos en la API de NowGrod.ia.
namespace vet_api_Net.Utilities;

public class HistoriaClinicaAnalizador : IHistoriaClinicaAnalizador
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApiSettingsOptions _options;
    private readonly ILogger<HistoriaClinicaAnalizador> _logger;

    public HistoriaClinicaAnalizador(
        IHttpClientFactory httpClientFactory, 
        IOptions<ApiSettingsOptions> options,
        ILogger<HistoriaClinicaAnalizador> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<ResumenClinicoIAResponseDTO> GenerarAnalisisClinicoAsync(Mascota mascota, List<Consulta> consultas, List<Vacuna> vacunas, string? resumenAnterior = null)
    {
        var apiKey = _options.GroqApiKey;
        var apiUrl = _options.GroqApiUrl;

        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogError("Groq API Key no está configurada en ApiSettings.");
            throw new InvalidOperationException(ResponseMessagesGroqServices.GroqApiKeyError);
        }

        if (string.IsNullOrEmpty(apiUrl))
        {
            _logger.LogError("Groq API URL no está configurada en ApiSettings.");
            throw new InvalidOperationException(ResponseMessagesGroqServices.GroqApiError);
        }

        try
        {
            var response = await ConsultarGroqApiAsync(apiKey, apiUrl, mascota, consultas, vacunas, resumenAnterior);
            if (response == null)
            {
                throw new InvalidOperationException(ResponseMessagesGroqServices.GroqResponseInvalid);
            }
            _logger.LogInformation("Análisis clínico generado con éxito mediante NowGrod.ia API.");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar el análisis clínico con NowGrod.ia API.");
            throw;
        }
    }

    private async Task<ResumenClinicoIAResponseDTO?> ConsultarGroqApiAsync(
        string apiKey, 
        string apiUrl, 
        Mascota mascota, 
        List<Consulta> consultas, 
        List<Vacuna> vacunas,
        string? resumenAnterior)
    {
        using var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var requestUrl = apiUrl.Replace("/chat", "/vet/analizar-historial");

        var requestBody = new
        {
            mascota = new
            {
                nombre = mascota.Nombre,
                especie = mascota.Especie,
                raza = mascota.Raza,
                sexo = mascota.Sexo,
                esterilizado = mascota.Esterilizado,
                fechaNacimiento = mascota.FechaNacimiento,
                peso = mascota.Peso,
                alergias = mascota.Alergias,
                condicionesMedicas = mascota.CondicionesMedicas
            },
            consultas = consultas.Select(c => new
            {
                fechaConsulta = c.FechaConsulta,
                sintomas = c.Sintomas,
                diagnostico = c.Diagnostico,
                tratamiento = c.Tratamiento,
                pesoActual = c.PesoActual
            }).ToList(),
            vacunas = vacunas.Select(v => new
            {
                producto = v.Producto != null ? new { nombre = v.Producto.Nombre } : null,
                fechaVacunacion = v.FechaVacunacion,
                proximaDosis = v.ProximaDosis
            }).ToList(),
            resumenAnterior = resumenAnterior
        };

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody, jsonOptions), Encoding.UTF8, "application/json");

        var response = await client.PostAsync(requestUrl, content);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Error en respuesta de NowGrod.ia API: {StatusCode}. Detalles: {Error}", response.StatusCode, errorContent);
            return null;
        }

        var responseString = await response.Content.ReadAsStringAsync();
        
        var deserializeOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<ResumenClinicoIAResponseDTO>(responseString, deserializeOptions);
    }
}
