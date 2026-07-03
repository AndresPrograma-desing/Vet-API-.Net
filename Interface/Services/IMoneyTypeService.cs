using vet_api_Net.Data;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Models;
using DTOs;

namespace vet_api_Net.Interfaze.Services;

public interface IMoneyTypeService
{
    Task<MoneyTypesDTO?> GetMoneyTypeAsync();
    Task<MoneyTypesDTO?> UpdateMoneyTypeAsync(MoneyTypesDTO money_name);
    Task<RequestDollarBcvDTO> GetTasaDollarBcvAsync();
    Task<RequestDollarBcvDTO> UpdateBcvDollarPriceAsync(decimal price);
}