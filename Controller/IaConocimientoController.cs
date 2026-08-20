using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DTOs;
using vet_api_Net.Routes;
using vet_api_Net.Constants;
using Microsoft.Extensions.Options;
using vet_api_Net.Infrastructure.Configuration;

namespace vet_api_Net.Controllers;

//Describe: Controlador HTTP para gestionar la base de conocimiento y reglas de comportamiento de la IA por categorías (Fowarded to Node.js API).
[ApiController]
[Route("api/[controller]")]
public class IaConocimientoController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApiSettingsOptions _options;
    private readonly ILogger<IaConocimientoController> _logger;

    public IaConocimientoController(IHttpClientFactory httpClientFactory, IOptions<ApiSettingsOptions> options, ILogger<IaConocimientoController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    [HttpGet(Endpoints.IaConocimiento.Categoria)]
    public async Task<ActionResult<IaConocimientoResponseDTO>> GetByCategoria(string categoria)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(categoria))
            {
                return BadRequest(new { error = ResponseMessagesIaConocimiento.CategoryRequired });
            }

            if (string.IsNullOrEmpty(_options.GroqApiUrl) || string.IsNullOrEmpty(_options.GroqApiKey))
            {
                _logger.LogError("NowGrod.ia no está configurada (GroqApiUrl/GroqApiKey faltante).");
                return StatusCode(503, new { error = ResponseMessagesGroqServices.ServiceUnavailable });
            }

            using var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.GroqApiKey);

            var requestUrl = _options.GroqApiUrl.Replace("/chat", $"/conocimientos/ia-config/{categoria.ToLower().Trim()}");

            HttpResponseMessage response;
            try
            {
                response = await client.GetAsync(requestUrl);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "No se pudo establecer conexión con NowGrod.ia en la URL: {RequestUrl}", requestUrl);
                return StatusCode(503, new { error = ResponseMessagesGroqServices.ServiceUnavailable });
            }

            if (!response.IsSuccessStatusCode)
            {
                return await HandleErrorResponseAsync(response);
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<IaConocimientoResponseDTO>(responseString) ?? new IaConocimientoResponseDTO();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al consultar la base de conocimiento de NowGrod.ia.");
            return StatusCode(503, new { error = ResponseMessagesGroqServices.ServiceUnavailable });
        }
    }

    [HttpPost(Endpoints.IaConocimiento.Guardar)]
    public async Task<ActionResult<IaConocimientoResponseDTO>> Guardar([FromBody] IaConocimientoSaveDTO dto)
    {
        try
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Categoria))
            {
                return BadRequest(new { error = ResponseMessagesIaConocimiento.CategoryRequired });
            }

            if (string.IsNullOrEmpty(_options.GroqApiUrl) || string.IsNullOrEmpty(_options.GroqApiKey))
            {
                _logger.LogError("NowGrod.ia no está configurada (GroqApiUrl/GroqApiKey faltante).");
                return StatusCode(503, new { error = ResponseMessagesGroqServices.ServiceUnavailable });
            }

            using var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.GroqApiKey);

            var requestUrl = _options.GroqApiUrl.Replace("/chat", "/conocimientos/ia-config");
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var content = new StringContent(JsonSerializer.Serialize(dto, jsonOptions), Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            try
            {
                response = await client.PostAsync(requestUrl, content);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "No se pudo establecer conexión con NowGrod.ia en la URL: {RequestUrl}", requestUrl);
                return StatusCode(503, new { error = ResponseMessagesGroqServices.ServiceUnavailable });
            }

            if (!response.IsSuccessStatusCode)
            {
                return await HandleErrorResponseAsync(response);
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<IaConocimientoResponseDTO>(responseString);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al guardar la base de conocimiento de NowGrod.ia.");
            return StatusCode(503, new { error = ResponseMessagesGroqServices.ServiceUnavailable });
        }
    }

    private async Task<ActionResult> HandleErrorResponseAsync(HttpResponseMessage response)
    {
        var errorContent = await response.Content.ReadAsStringAsync();
        _logger.LogError("Error en respuesta de NowGrod.ia: {StatusCode}. Detalles: {Error}", response.StatusCode, errorContent);

        return StatusCode(503, new { error = ResponseMessagesGroqServices.ServiceUnavailable });
    }
}
