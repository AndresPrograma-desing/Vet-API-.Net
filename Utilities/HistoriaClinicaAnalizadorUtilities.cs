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

//Describe: Analizador clínico que delegaba el procesamiento de historiales médicos en la API de NowGrod.ia.
//Mockeado temporalmente (devuelve un resumen placeholder sin red) porque NowGrod.ia va a dejar de usarse; ConsultarGroqApiAsync queda sin uso hasta removerlo junto con el servicio.
namespace vet_api_Net.Utilities;

public class HistoriaClinicaAnalizadorUtilities : IHistoriaClinicaAnalizadorUtilities
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApiSettingsOptions _options;
    private readonly ILogger<HistoriaClinicaAnalizadorUtilities> _logger;

    public HistoriaClinicaAnalizadorUtilities(
        IHttpClientFactory httpClientFactory,
        IOptions<ApiSettingsOptions> options,
        ILogger<HistoriaClinicaAnalizadorUtilities> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    public Task<ResumenClinicoIAResponseDTO> GenerarAnalisisClinicoAsync(Mascota mascota, List<Consulta> consultas, List<PetVaccination> vaccinations, string? resumenAnterior = null)
    {
        _logger.LogInformation("NowGrod.ia está mockeado: se devuelve un resumen placeholder sin consultar el servicio externo.");

        var mockResponse = new ResumenClinicoIAResponseDTO
        {
            ResumenIa = "Resumen por IA no disponible en este momento.",
            AlertasRiesgoIa = string.Empty,
            SugerenciasIa = string.Empty
        };

        return Task.FromResult(mockResponse);
    }

    private async Task<ResumenClinicoIAResponseDTO?> ConsultarGroqApiAsync(
        string apiKey, 
        string apiUrl, 
        Mascota mascota,
        List<Consulta> consultas,
        List<PetVaccination> vaccinations,
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
                especie = mascota.Especie.Nombre,
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
            vacunas = vaccinations.Select(v => new
            {
                producto = v.Vaccine != null ? new { nombre = v.Vaccine.Name } : null,
                fechaVacunacion = v.ApplicationDate,
                proximaDosis = v.NextDoseDate
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
