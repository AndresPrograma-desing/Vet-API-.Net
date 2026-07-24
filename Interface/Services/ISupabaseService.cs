using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace vet_api_Net.Interfaze.Services;

public interface ISupabaseService
{
    Task<string> UploadAvatarAsync(IFormFile file, int userId);
    Task<string> GetUrlAvatar(string userId);
}