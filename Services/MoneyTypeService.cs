using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DTOs;
using vet_api_Net.Data;
using vet_api_Net.Models;
namespace vet_api_Net.Interfaze.Services;
using vet_api_Net.Constants;
public class MoneyTypeService : IMoneyTypeService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public MoneyTypeService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<MoneyTypesDTO?> GetMoneyTypeAsync()
    {
        var moneyType = await _context.MoneyTypes.FirstOrDefaultAsync();
        if (moneyType == null) return null;

        int targetId = _configuration.GetValue<int>("BcvSettings:TargetId", 1);

        if(moneyType.Id != targetId)
        {
            throw new Exception(ResponseMessagesMoneyTypes.InvalidId);
        }
        return new MoneyTypesDTO
        {
            Id = moneyType.Id,
            MoneyName = moneyType.MoneyName
        };
    }

    public async Task<MoneyTypesDTO?> UpdateMoneyTypeAsync(MoneyTypesDTO money_name)
    {
        var existingMoneyType = await _context.MoneyTypes.FirstOrDefaultAsync();

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
            _context.MoneyTypes.Add(existingMoneyType);
        }
        else
        {
            existingMoneyType.MoneyName = money_name.MoneyName;
            _context.MoneyTypes.Update(existingMoneyType);
        }

        await _context.SaveChangesAsync();

        return new MoneyTypesDTO
        {
            Id = existingMoneyType.Id,
            MoneyName = existingMoneyType.MoneyName
        };
    }
    public async Task<RequestDollarBcvDTO> GetTasaDollarBcvAsync()
    {
        var moneyType = await _context.MoneyTypes.FirstOrDefaultAsync(m => m.DollarPersistence == "USD");
        
       
        if (moneyType == null)
        {
            return new RequestDollarBcvDTO
            {
                MoneyType = "USD",
                BcvDollar = 0m
            };
        }

        return new RequestDollarBcvDTO
        {
            MoneyType = moneyType.DollarPersistence,
            BcvDollar = moneyType.BcvDollar
        };
    }
}