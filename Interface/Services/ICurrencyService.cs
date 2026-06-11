using System.Threading.Tasks;

namespace vet_api_Net.Interfaze.Services;

public interface ICurrencyService
{
    Task<decimal> ConvertPriceAsync(decimal originalPriceInUsd);
    Task<decimal> ConvertToUsdAsync(decimal currentPrice);
}