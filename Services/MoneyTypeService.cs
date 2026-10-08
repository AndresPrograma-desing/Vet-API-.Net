using System;
using System.Globalization;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Models;
namespace vet_api_Net.Interfaze.Services;
using vet_api_Net.Constants;
public class MoneyTypeService : IMoneyTypeService
{
    private readonly IMoneyTypeRepository _repository;
    private readonly IConfiguration _configuration;

    public MoneyTypeService(IMoneyTypeRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    public async Task<MoneyTypesDTO?> GetMoneyTypeAsync()
    {
        var moneyType = await _repository.GetFirstAsync();
        if (moneyType == null) return null;

        int targetId = _configuration.GetValue<int>("BcvSettings:TargetId", 1);

        if(moneyType.Id != targetId)
        {
            throw new Exception(ResponseMessagesMoneyTypes.InvalidId);
        }
        return new MoneyTypesDTO
        {
            Id = moneyType.Id,
            MoneyName = moneyType.MoneyName,
            TypeMoney = moneyType.MoneyName?.ToUpper() ?? "USD",
            TasaBcv = moneyType.BcvDollar > 0 ? moneyType.BcvDollar : 1.0m
        };
    }

    public async Task<MoneyTypesDTO?> UpdateMoneyTypeAsync(MoneyTypesDTO money_name)
    {
        var existingMoneyType = await _repository.GetFirstAsync();

        int targetId = _configuration.GetValue<int>("BcvSettings:TargetId", 1);

        if(existingMoneyType != null && existingMoneyType.Id != targetId)
        {
            throw new Exception(ResponseMessagesMoneyTypes.ErrorUpdate);
        }
        if (existingMoneyType == null)
        {
            existingMoneyType = new MoneyType
            {
                MoneyName = money_name.MoneyName
            };
            _repository.Add(existingMoneyType);
        }
        else
        {
            existingMoneyType.MoneyName = money_name.MoneyName;
            _repository.Update(existingMoneyType);
        }

        await _repository.SaveChangesAsync();

        return new MoneyTypesDTO
        {
            Id = existingMoneyType.Id,
            MoneyName = existingMoneyType.MoneyName
        };
    }
public async Task<RequestDollarBcvDTO> GetTasaDollarBcvAsync()
{
    var moneyType = await _repository.GetByDollarPersistenceAsync("USD");
    
    if (moneyType == null || moneyType.BcvDollar == 0m)
    {
        return new RequestDollarBcvDTO
        {
            MoneyType = moneyType?.DollarPersistence ?? "USD",
            BcvDollar = ResponseMessagesMoneyTypes.BcvFallen,
            Message = ResponseMessagesMoneyTypes.GetTasaBcvError
        };
    }

    return new RequestDollarBcvDTO
    {
        MoneyType = moneyType.DollarPersistence,
        BcvDollar = moneyType.BcvDollar.ToString("F2", CultureInfo.InvariantCulture),
        Message = ResponseMessagesMoneyTypes.BcvRequestSuccess
    };
}

    public async Task<RequestDollarBcvDTO> UpdateBcvDollarPriceAsync(decimal price)
    {
        if (price <= 0)
        {
            throw new ArgumentException(ResponseMessagesMoneyTypes.InvalidPrice);
        }

        int targetId = _configuration.GetValue<int>("BcvSettings:TargetId", 1);
        var moneyEntry = await _repository.GetByIdAsync(targetId);

        if (moneyEntry == null)
        {
            moneyEntry = new MoneyType
            {
                Id = targetId,
                BcvDollar = price,
                DollarPersistence = "USD",
                Fecha = DateTime.Now
            };
            _repository.Add(moneyEntry);
        }
        else
        {
            moneyEntry.BcvDollar = price;
            moneyEntry.DollarPersistence = "USD";
            moneyEntry.Fecha = DateTime.Now;
            _repository.Update(moneyEntry);
        }

        await _repository.SaveChangesAsync();

        return new RequestDollarBcvDTO
        {
            MoneyType = "USD",
            BcvDollar = price.ToString("F2", CultureInfo.InvariantCulture),
            Message = ResponseMessagesMoneyTypes.BcvRequestSuccess
        };
    }
}