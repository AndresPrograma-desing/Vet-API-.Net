using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using DTOs;
using vet_api_Net.Models;
using vet_api_Net.Interfaces.Services;
using vet_api_Net.Interfaces.Repositories;
using vet_api_Net.Constants;

namespace vet_api_Net.Services
{
    public class MoneyTypeService : IMoneyTypeService
    {
        private readonly IMoneyTypeRepository _moneyTypeRepository;
        private readonly IConfiguration _configuration;

        public MoneyTypeService(IMoneyTypeRepository moneyTypeRepository, IConfiguration configuration)
        {
            _moneyTypeRepository = moneyTypeRepository;
            _configuration = configuration;
        }

        public async Task<MoneyTypesDTO?> GetMoneyTypeAsync()
        {
            var moneyType = await _moneyTypeRepository.GetFirstOrDefaultAsync();
            if (moneyType == null) return null;

            int targetId = _configuration.GetValue<int>("BcvSettings:TargetId", 1);

            if (moneyType.Id != targetId)
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
            var existingMoneyType = await _moneyTypeRepository.GetFirstOrDefaultAsync();

            int targetId = _configuration.GetValue<int>("BcvSettings:TargetId", 1);

            if (existingMoneyType != null && existingMoneyType.Id != targetId)
            {
                throw new Exception(ResponseMessagesMoneyTypes.ErrorUpdate);
            }
            if (existingMoneyType == null)
            {
                existingMoneyType = new MoneyType
                {
                    MoneyName = money_name.MoneyName
                };
                _moneyTypeRepository.Add(existingMoneyType);
            }
            else
            {
                existingMoneyType.MoneyName = money_name.MoneyName;
                _moneyTypeRepository.Update(existingMoneyType);
            }

            await _moneyTypeRepository.SaveChangesAsync();

            return new MoneyTypesDTO
            {
                Id = existingMoneyType.Id,
                MoneyName = existingMoneyType.MoneyName
            };
        }

        public async Task<RequestDollarBcvDTO> GetTasaDollarBcvAsync()
        {
            var moneyType = await _moneyTypeRepository.GetDollarPersistenceAsync();

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
}