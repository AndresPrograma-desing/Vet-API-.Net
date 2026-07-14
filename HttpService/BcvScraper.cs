using HtmlAgilityPack;
using System.Globalization;
using vet_api_Net.Constants;
using Microsoft.Extensions.Logging;
using vet_api_Net.Interfaze.Repositories;
namespace vet_api_Net.HttpServices;

public class BcvScraper : IBcvScraper 
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ISystemConfigRepository _systemConfigRepository;
    private readonly ILogger<BcvScraper> _logger;
    private readonly Microsoft.Extensions.DependencyInjection.IServiceScopeFactory _scopeFactory;
    
    public BcvScraper(
    IHttpClientFactory httpClientFactory,
     ILogger<BcvScraper> logger, 
     Microsoft.Extensions.DependencyInjection.IServiceScopeFactory scopeFactory, 
     ISystemConfigRepository systemConfigRepository
     )
    {
        _httpClientFactory = httpClientFactory;
        _systemConfigRepository = systemConfigRepository;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task<decimal?> ObtenerPrecioBcvAsync()
    {
        try
        { 
            string? bcvUrl;
            using (var scope = _scopeFactory.CreateScope())
            {
                var SystemConfig = await _systemConfigRepository.GetSystemConfigAsync();

                bcvUrl = SystemConfig?.BcvApiUrl;
            }

            if (string.IsNullOrWhiteSpace(bcvUrl))
            {
                _logger.LogWarning("La URL del BCV no está configurada en la tabla SystemConfigs.");
                return null;
            }

            var client = _httpClientFactory.CreateClient("BcvClient");
            var html = await client.GetStringAsync(bcvUrl); 
            
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var nodo = doc.DocumentNode.SelectSingleNode("//div[@id='dolar']//strong");

            if (nodo != null)
            {
                string valorTexto = nodo.InnerText.Trim();
                return decimal.Parse(valorTexto, new CultureInfo("es-VE"));
            }
            _logger.LogWarning("No se encontró el precio del BCV.");
            return null;
        }
        catch (Exception ex)
        { 
            _logger.LogError(ex, "Error al obtener el precio del BCV." + ex.Message);
            return null;
        }
    }
}