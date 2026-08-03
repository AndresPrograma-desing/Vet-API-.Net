using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Utilities;
using vet_api_Net.Infrastructure.Configuration;
using DTOs;
using vet_api_Net.Services;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Constants;
using Microsoft.Extensions.Options;

namespace vet_api_Net.Utilities;

// CurrencyService se encarga de convertir precios entre USD y la moneda local (VES) según la configuración y el tipo de cambio actual.

public class CurrencyUtilities : ICurrencyUtilities
{
    private readonly IProductRepository _repository;
    private readonly IConfiguration _configuration;
    private readonly ApiSettingsOptions _apiSettings;
    private readonly IMoneyTypeService _moneyTypeService;
    private readonly int _targetId;

    public CurrencyUtilities(IProductRepository repository, IConfiguration configuration, IOptions<ApiSettingsOptions> apiSettingsOptions, IMoneyTypeService moneyTypeService)
    {
        _repository = repository;
        _configuration = configuration;
        _apiSettings = apiSettingsOptions.Value;
        _moneyTypeService = moneyTypeService;
        _targetId = _configuration.GetValue<int>("BcvSettings:TargetId", 1);
    }

    public async Task<decimal> ConvertPriceAsync(decimal originalPriceInUsd)
    {
        var Money = await _moneyTypeService.GetMoneyTypeAsync();

        if (Money == null || (Money.TypeMoney == null && Money.TasaBcv == 0))
        {
            throw new Exception(ResponseMessagesUtilties.ErroConverMoney);
        }
        if (Money.TypeMoney == _apiSettings.VES)
        {
            return originalPriceInUsd * Money.TasaBcv;
        }

        return originalPriceInUsd;
    }

    public async Task<decimal> ConvertToUsdAsync(decimal currentPrice)
    {
        var Money = await _moneyTypeService.GetMoneyTypeAsync();

        if (Money == null || (Money.TypeMoney == null && Money.TasaBcv == 0))
        {
            throw new Exception(ResponseMessagesUtilties.ErroConverMoney);
        }
        if (Money.TypeMoney == _apiSettings.VES && Money.TasaBcv > 0)
        {
            return currentPrice / Money.TasaBcv;
        }
        return currentPrice;
    }

    //   private async Task<MoneyTypesDTO> GetMoney()
    //     {
    //         var Money = await _repository.GetMoneyTypeByIdAsync(_targetId);

    //         string typeMoney = Money?.MoneyName?.ToUpper() ?? "USD";
    //         decimal tasaBcv = Money?.BcvDollar ?? 1.0m;

    //         return new MoneyTypesDTO 
    //         {
    //             TypeMoney = typeMoney,
    //             TasaBcv = tasaBcv
    //         };
    //     }
}