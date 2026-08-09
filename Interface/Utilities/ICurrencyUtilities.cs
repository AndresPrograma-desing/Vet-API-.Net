using System.Threading.Tasks;
using DTOs;

namespace vet_api_Net.Interfaze.Utilities;

public interface ICurrencyUtilities
{
    Task<decimal> ConvertPriceAsync(decimal originalPriceInUsd);
    Task<decimal> ConvertToUsdAsync(decimal currentPrice);
    Task<MoneyTypesDTO> GetActiveMoneyTypeAsync();
    decimal ConvertPrice(decimal originalPriceInUsd, MoneyTypesDTO money);
}