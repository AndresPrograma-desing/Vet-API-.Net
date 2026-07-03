using HtmlAgilityPack;
using System.Globalization;
using vet_api_Net.Constants;
using Microsoft.Extensions.Logging;

namespace vet_api_Net.HttpServices;

public class BcvScraper : IBcvScraper 
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<BcvScraper> _logger;
    
    public BcvScraper(IHttpClientFactory httpClientFactory, ILogger<BcvScraper> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<decimal?> ObtenerPrecioBcvAsync()
    {
        try
        { 
            var client = _httpClientFactory.CreateClient("BcvClient");
            var html = await client.GetStringAsync("/"); 
            
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