using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

    public IaConocimientoController(IHttpClientFactory httpClientFactory, IOptions<ApiSettingsOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
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

            if (string.IsNullOrEmpty(_options.GroqApiUrl))
            {
                return StatusCode(500, new { error = ResponseMessagesGroqServices.GroqApiError });
            }

            if (string.IsNullOrEmpty(_options.GroqApiKey))
            {
                return StatusCode(500, new { error = ResponseMessagesGroqServices.GroqApiKeyError });
            }

            using var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.GroqApiKey);

            var requestUrl = _options.GroqApiUrl.Replace("/chat", $"/conocimientos/ia-config/{categoria.ToLower().Trim()}");
            
            HttpResponseMessage response;
            try
            {
                response = await client.GetAsync(requestUrl);
            }
            catch (HttpRequestException)
            {
                return StatusCode(503, new { error = ResponseMessagesGroqServices.GroqApiError });
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
            return StatusCode(500, new { error = ex.Message });
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

            if (string.IsNullOrEmpty(_options.GroqApiUrl))
            {
                return StatusCode(500, new { error = ResponseMessagesGroqServices.GroqApiError });
            }

            if (string.IsNullOrEmpty(_options.GroqApiKey))
            {
                return StatusCode(500, new { error = ResponseMessagesGroqServices.GroqApiKeyError });
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
            catch (HttpRequestException)
            {
                return StatusCode(503, new { error = ResponseMessagesGroqServices.GroqApiError });
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
            return StatusCode(500, new { error = ex.Message });
        }
    }

    private async Task<ActionResult> HandleErrorResponseAsync(HttpResponseMessage response)
    {
        var errorContent = await response.Content.ReadAsStringAsync();
        var contentType = response.Content.Headers.ContentType?.MediaType;

        if ((contentType != null && contentType.Contains("text/html")) || 
            (!string.IsNullOrEmpty(errorContent) && errorContent.TrimStart().StartsWith("<")))
        {
            return StatusCode((int)response.StatusCode, new { error = ResponseMessagesGroqServices.GroqApiError });
        }

        try
        {
            using var doc = JsonDocument.Parse(errorContent);
            if (doc.RootElement.TryGetProperty("error", out var errProp))
            {
                var errStr = errProp.GetString();
                if (!string.IsNullOrEmpty(errStr))
                {
                    return StatusCode((int)response.StatusCode, new { error = errStr });
                }
            }
        }
        catch
        {
            // El contenido no es JSON válido
        }

        return StatusCode((int)response.StatusCode, new { error = errorContent });
    }
}
