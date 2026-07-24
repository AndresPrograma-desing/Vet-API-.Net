using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Utilities;
using vet_api_Net.Infrastructure.Configuration;
using DTOs;
using vet_api_Net.Services;
using vet_api_Net.Constants;
using Microsoft.Extensions.Options;
using vet_api_Net.Interface.Utilities;
using Microsoft.AspNetCore.Http;

namespace vet_api_Net.Utilities;

public class AvatarUrlRequestUtilities : IAvatarUrlRequestUtilities
{
    private readonly ISupabaseService _supabaseService;
    private readonly IUserService _userService;
    private readonly IUsersRepository _userRepository;

    public AvatarUrlRequestUtilities(ISupabaseService supabaseService, IUserService userService, IUsersRepository userRepository)
    {
        _supabaseService = supabaseService;
        _userService = userService;
        _userRepository = userRepository;
    }

    public async Task<string> GetUrlAvatar(int userId, string url)
    {
       var user = await _userRepository.GetByIdAsync(userId);
       if (user == null)
       {
           return url;
       }
       if (string.IsNullOrEmpty(user.AvatarUrl))
       {
           return url;
       }
       return user.AvatarUrl;
    }

    // public async Task<string> UpdateAvatar(int userId, IFormFile file)
    // {
    //     return await _supabaseService.UploadAvatarAsync(file, userId);
    // }

    // public async Task<string> DeleteAvatar(string userId)
    // {
    //     return await _supabaseService.DeleteAvatar(userId);
    // }
}

