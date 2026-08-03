using System.Threading.Tasks;

namespace vet_api_Net.Interfaze.Utilities;

public interface ICurrencyUtilities
{
    Task<decimal> ConvertPriceAsync(decimal originalPriceInUsd);
    Task<decimal> ConvertToUsdAsync(decimal currentPrice);
}