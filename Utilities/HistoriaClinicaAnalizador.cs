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

//Describe: Analizador clínico experto que utiliza la API de Groq (Llama 3) para procesar historiales médicos en tiempo real a través del Options Pattern.
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
        var model = _options.GroqModel;

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

        if (string.IsNullOrEmpty(model))
        {
            _logger.LogError("Groq Model no está configurado en ApiSettings.");
            throw new InvalidOperationException(ResponseMessagesGroqServices.GroqApiError);
        }

        try
        {
            var response = await ConsultarGroqApiAsync(apiKey, apiUrl, model, mascota, consultas, vacunas, resumenAnterior);
            if (response == null)
            {
                throw new InvalidOperationException(ResponseMessagesGroqServices.GroqResponseInvalid);
            }
            _logger.LogInformation("Análisis clínico generado con éxito mediante Groq API.");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar el análisis clínico con Groq API.");
            throw;
        }
    }

    private async Task<ResumenClinicoIAResponseDTO?> ConsultarGroqApiAsync(
        string apiKey, 
        string apiUrl, 
        string model, 
        Mascota mascota, 
        List<Consulta> consultas, 
        List<Vacuna> vacunas,
        string? resumenAnterior)
    {
        using var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var prompt = ConstruirPrompt(mascota, consultas, vacunas, resumenAnterior);

        var requestBody = new
        {
            model = model,
            messages = new[]
            {
                new
                {
                    role = "system",
                    content = ResponseMessagesGroqServices.GroqPromts.GroqResponseHistorial
                },
                new
                {
                    role = "user",
                    content = prompt
                }
            },
            response_format = new
            {
                type = "json_object"
            },
            temperature = 0.3
        };

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody, jsonOptions), Encoding.UTF8, "application/json");

        var response = await client.PostAsync(apiUrl, content);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Error en respuesta de Groq API: {StatusCode}. Detalles: {Error}", response.StatusCode, errorContent);
            return null;
        }

        var responseString = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseString);
        var contentResult = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        if (string.IsNullOrEmpty(contentResult)) return null;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("=================== RESPUESTA DE GROQ ===================");
        Console.WriteLine(contentResult);
        Console.WriteLine("=========================================================");
        Console.ResetColor();

        var deserializeOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<ResumenClinicoIAResponseDTO>(contentResult, deserializeOptions);
    }

    private string ConstruirPrompt(Mascota mascota, List<Consulta> consultas, List<Vacuna> vacunas, string? resumenAnterior)
    {
        var sb = new StringBuilder();
        
        if (!string.IsNullOrEmpty(resumenAnterior))
        {
            sb.AppendLine("=== ANÁLISIS DE EVOLUCIÓN ===");
            sb.AppendLine($"Aquí tienes el resumen clínico anterior de {mascota.Nombre}:");
            sb.AppendLine(resumenAnterior);
            sb.AppendLine("Instrucción crucial: Evalúa cómo ha progresado el estado de salud, peso o tratamientos en comparación con las nuevas consultas que verás abajo.");
            sb.AppendLine("=====================================\n");
        }

        sb.AppendLine("Analiza los siguientes datos clínicos actuales de la mascota:");
        sb.AppendLine($"- Nombre: {mascota.Nombre}");
        sb.AppendLine($"- Especie: {mascota.Especie}");
        sb.AppendLine($"- Raza: {mascota.Raza ?? "No especificada"}");
        sb.AppendLine($"- Sexo: {mascota.Sexo}");
        sb.AppendLine($"- Esterilizado: {(mascota.Esterilizado == true ? "Sí" : "No")}");
        sb.AppendLine($"- Fecha de Nacimiento: {mascota.FechaNacimiento?.ToString("dd-MM-yyyy") ?? "Desconocida"}");
        sb.AppendLine($"- Peso actual registrado: {mascota.Peso?.ToString() ?? "No registrado"} kg");
        sb.AppendLine($"- Alergias conocidas: {mascota.Alergias ?? "Ninguna"}");
        sb.AppendLine($"- Condiciones médicas: {mascota.CondicionesMedicas ?? "Sano"}");

        if (consultas != null && consultas.Count > 0)
        {
            sb.AppendLine("\nHistorial de Consultas Médicas Recientes (ordenadas de más reciente a más antigua):");
            foreach (var c in consultas.OrderByDescending(c => c.FechaConsulta).Take(5))
            {
                sb.AppendLine($"- Fecha: {c.FechaConsulta:dd-MM-yyyy} | Síntomas: {c.Sintomas} | Diagnóstico: {c.Diagnostico ?? "Sin diagnóstico aún"} | Tratamiento: {c.Tratamiento ?? "Ninguno"} | Peso registrado en consulta: {c.PesoActual ?? mascota.Peso} kg");
            }
        }
        else
        {
            sb.AppendLine("\nNo hay consultas médicas previas registradas.");
        }

        if (vacunas != null && vacunas.Count > 0)
        {
            sb.AppendLine("\nHistorial de Inmunización:");
            foreach (var v in vacunas)
            {
                sb.AppendLine($"- Vacuna: {v.Producto?.Nombre ?? "Vacuna"} | Fecha de aplicación: {v.FechaVacunacion:dd-MM-yyyy} | Próxima dosis programada: {v.ProximaDosis?.ToString("dd-MM-yyyy") ?? "No programada"}");
            }
        }
        else
        {
            sb.AppendLine("\nNo registra vacunas administradas.");
        }

        return sb.ToString();
    }
}
