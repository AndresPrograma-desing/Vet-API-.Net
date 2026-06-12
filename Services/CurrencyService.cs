using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using vet_api_Net.Interfaces.Repositories;
using vet_api_Net.Interfaces.Services;

namespace vet_api_Net.Services;
        // CurrencyService se encarga de convertir precios entre USD y la moneda local (VES) segÃºn la configuraciÃ³n y el tipo de cambio actual.
public class CurrencyService : ICurrencyService
{
    private readonly IProductRepository _repository;
    private readonly IConfiguration _configuration;
    private readonly int _targetId;

    public CurrencyService(IProductRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
        _targetId = _configuration.GetValue<int>("BcvSettings:TargetId", 1);
    }

    public async Task<decimal> ConvertPriceAsync(decimal originalPriceInUsd)
    {
        var moneyType = await _repository.GetMoneyTypeByIdAsync(_targetId);
        string typeMoney = moneyType?.MoneyName?.ToUpper() ?? "USD";
        decimal tasaBcv = moneyType?.BcvDollar ?? 1.0m;

        if (typeMoney == "VES")
        {
            return originalPriceInUsd * tasaBcv;
        }

        return originalPriceInUsd;
    }

    public async Task<decimal> ConvertToUsdAsync(decimal currentPrice)
{
    var moneyType = await _repository.GetMoneyTypeByIdAsync(_targetId);
    string typeMoney = moneyType?.MoneyName?.ToUpper() ?? "USD";
    decimal tasaBcv = moneyType?.BcvDollar ?? 1.0m;

    if (typeMoney == "VES" && tasaBcv > 0)
    {
        return currentPrice / tasaBcv;
    }
    return currentPrice;
}
}