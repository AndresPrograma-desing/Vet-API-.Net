using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DTOs;
using vet_api_Net.Infrastructure.Configuration;
using vet_api_Net.Interfaze.Services;

//Describe:
// Cliente HTTP para el SDK externo ContextHub (login vía iframe + ticket de un solo uso).
// Canjea el ticket por {id, email} real del usuario y provee el alta de usuarios en ContextHub
// cuando se crean desde el panel admin del backend, para mantener ambos lados sincronizados.

namespace vet_api_Net.HttpServices;

public class ContextHubClient : IContextHubClient
{
    private readonly HttpClient _httpClient;
    private readonly ContextHubOptions _options;
    private readonly ILogger<ContextHubClient> _logger;

    public ContextHubClient(HttpClient httpClient, IOptions<ContextHubOptions> options, ILogger<ContextHubClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<ContextHubVerifyResponseDTO?> VerifyTicketAsync(string ticket)
    {
        var url = $"{_options.BaseUrl.TrimEnd('/')}/api/v1/sdk/verify";

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        var payload = new ContextHubVerifyTicketRequest { Ticket = ticket };
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        if (!string.IsNullOrWhiteSpace(_options.AnonKey))
        {
            request.Headers.Add("apikey", _options.AnonKey);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.AnonKey);
        }
        request.Headers.Add("x-sdk-client-secret", _options.ClientSecret);

        try
        {
            var response = await _httpClient.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("ContextHub verify-ticket falló con status {StatusCode}: {Body}", response.StatusCode, body);
                return null;
            }

            return JsonSerializer.Deserialize<ContextHubVerifyResponseDTO>(body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al conectar con ContextHub para verificar ticket.");
            return null;
        }
    }

    public async Task<ContextHubCreateUserResponseDTO?> CreateUserAsync(string email, string password)
    {
        var url = $"{_options.BaseUrl.TrimEnd('/')}/context/hub/v1/provider/login/users/create";

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        var payload = new ContextHubCreateUserRequest { Email = email, Password = password };
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        request.Headers.Add("x-sdk-client-secret", _options.ClientSecret);

        try
        {
            var response = await _httpClient.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(body))
            {
                _logger.LogWarning("ContextHub create-user falló con status {StatusCode} y sin cuerpo de respuesta.", response.StatusCode);
                return null;
            }

            var result = JsonSerializer.Deserialize<ContextHubCreateUserResponseDTO>(body);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("ContextHub create-user falló con status {StatusCode}: {Body}", response.StatusCode, body);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al conectar con ContextHub para crear usuario.");
            return null;
        }
    }
}
