using System.Threading.Tasks;

namespace vet_api_Net.Interfaces.Services;

public interface ICurrencyService
{
    Task<decimal> ConvertPriceAsync(decimal originalPriceInUsd);
    Task<decimal> ConvertToUsdAsync(decimal currentPrice);
}