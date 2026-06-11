using HtmlAgilityPack;
using System.Globalization;
using vet_api_Net.Constants;

namespace vet_api_Net.HttpServices;

public class BcvScraper : IBcvScraper 
{
    private readonly IHttpClientFactory _httpClientFactory;
    public BcvScraper(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<decimal> ObtenerPrecioBcvAsync()
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
        }
        catch (Exception)
        { 
            throw new Exception(ResponseMessagesMoneyTypes.ScrapingError);
        }

        return 0;
    }
}