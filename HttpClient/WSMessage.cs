using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace vet_api_Net.Services.WSMessage
{
    public class WSMessage : IWSMessage
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WSMessage> _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IConfiguration _configuration;
        private readonly IWSMRepository _wsmRepository;

        public WSMessage(HttpClient httpClient, IConfiguration configuration, IWSMRepository wsmRepository, ILogger<WSMessage> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _wsmRepository = wsmRepository;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<bool> EnviarComprobanteAsync(WSMessageDTO datosEnvio)
        {
            try
            {
                string jsonBody = JsonSerializer.Serialize(datosEnvio, _jsonOptions);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                var apiData = await _wsmRepository.GetWSMessageAPIDataAsync();
                _httpClient.DefaultRequestHeaders.Add("x-api-key", apiData?.ApiKey ?? "");

                string apiUrl = _configuration["WSMessage:ApiUrl"]?.TrimEnd('/') ?? "";
                HttpResponseMessage response = await _httpClient.PostAsync($"{apiUrl}/send-invoice", content);

                if (response.IsSuccessStatusCode)
                { 
                    return true;
                }

                string errorResponse = await response.Content.ReadAsStringAsync(); 
                return false;
            }
            catch (Exception)
            { 
                return false;
            }
        }

        public async Task<bool> IniciarSesionAsync()
        {
            try
            {
                var apiData = await _wsmRepository.GetWSMessageAPIDataAsync();
                string dbClientId = apiData?.ClientId ?? "";

                if (string.IsNullOrWhiteSpace(dbClientId))
                { 
                    return false;
                }

                var payload = new WSInitSessionDTO { ClientId = dbClientId };
                string jsonBody = JsonSerializer.Serialize(payload, _jsonOptions);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
 
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-api-key", apiData?.ApiKey ?? "");

                string apiUrl = _configuration["WSMessage:ApiUrl"]?.TrimEnd('/') ?? "";
                HttpResponseMessage response = await _httpClient.PostAsync($"{apiUrl}/init-session", content);

                if (response.IsSuccessStatusCode)
                { 
                    return true;
                }

                string errorResponse = await response.Content.ReadAsStringAsync();
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<WSStatusResponseDTO?> ObtenerEstadoOSesionAsync()
        {
            try
            { 
                _httpClient.DefaultRequestHeaders.Clear();
                var apiData = await _wsmRepository.GetWSMessageAPIDataAsync();
                string dbClientId = apiData?.ClientId ?? "";

                if (string.IsNullOrWhiteSpace(dbClientId))
                {
                    return null;
                }

                _httpClient.DefaultRequestHeaders.Add("x-api-key", apiData?.ApiKey ?? "");

                string apiUrl = _configuration["WSMessage:ApiUrl"]?.TrimEnd('/') ?? "";
                HttpResponseMessage response = await _httpClient.GetAsync($"{apiUrl}/status/{dbClientId}");

                if (response.IsSuccessStatusCode)
                {
                    string contentJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<WSStatusResponseDTO>(contentJson, _jsonOptions);
                }

                string errorResponse = await response.Content.ReadAsStringAsync();
                return null;
            }
            catch (Exception)
            { 
                return null;
            }
        }
    }
}