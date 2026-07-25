using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Models;
using vet_api_Net.Interfaze.Repositories;
using DTOs;

namespace vet_api_Net.Repositories;

public class SystemConfigRespository: ISystemConfigRepository
{
    private readonly AppDbContext _context;

    public SystemConfigRespository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SystemConfig?> GetSystemConfigAsync()
    {
        return  await _context.SystemConfigs.FirstOrDefaultAsync();
    }

    public async Task UpdateResendConfigAsync(string apiKey, string fromEmail, string apiUrl)
    {
        var config = await _context.SystemConfigs.FirstOrDefaultAsync();
        if (config == null)
        {
            config = new SystemConfig
            {
                FrontendUrl = "https://happy-pets-web.vercel.app",
                BackendExternalUrl = "https://g27frlv5-5168.use2.devtunnels.ms/",
                BcvApiUrl = "https://www.bcv.org.ve",
                LastUpdated = DateTime.UtcNow
            };
            _context.SystemConfigs.Add(config);
        }

        config.ResendApiKey = apiKey;
        config.ResendFromEmail = fromEmail;
        config.ResendApiUrl = apiUrl;
        config.LastUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}