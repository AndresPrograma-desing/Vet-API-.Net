using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Models;
using vet_api_Net.Repositories;
using DTOs;
using Microsoft.Extensions.Logging;

namespace vet_api_Net.Repositories
{
    public class WSMRepository : IWSMRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<WSMRepository> _logger;


        public WSMRepository(AppDbContext context, ILogger<WSMRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<WSMessageDbDTO?> GetWSMessageAPIDataAsync()
        {
            var apiData = await _context.WSMessageAPIData.FirstOrDefaultAsync();

            if (apiData == null)
                return null;
         _logger.LogInformation("WSMessageAPIData retrieved successfully for ClientId: {ClientId}", apiData.ClientId);
            return new WSMessageDbDTO
            {
                ClientId = apiData.ClientId,
                ApiKey = apiData.ApiKey,
                Message = apiData.Message
            };
   
        }
    }
}