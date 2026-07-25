using System.Threading.Tasks;
using System.Collections.Generic;
using vet_api_Net.Models;
using DTOs;

namespace vet_api_Net.Interfaze.Repositories;

public interface ISystemConfigRepository
{
    Task<SystemConfig?> GetSystemConfigAsync();
    Task UpdateResendConfigAsync(string apiKey, string fromEmail, string apiUrl);
}