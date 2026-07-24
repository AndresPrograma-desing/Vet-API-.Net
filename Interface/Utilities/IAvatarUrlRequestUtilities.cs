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

namespace vet_api_Net.Interface.Utilities;

public interface IAvatarUrlRequestUtilities
{
    Task<string> GetUrlAvatar(int userId, string url);
    // Task<string> UpdateAvatar(int userId, IFormFile file);
    // Task<string> DeleteAvatar(string userId);
}