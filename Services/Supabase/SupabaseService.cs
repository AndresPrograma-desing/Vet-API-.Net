using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using vet_api_Net.Constants;
using vet_api_Net.Infrastructure.Configuration;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using SupabaseClient = global::Supabase.Client;
using SupabaseStorageOptions = global::Supabase.Storage.FileOptions;

namespace vet_api_Net.Services.Supabase
{
    public class SupabaseService : ISupabaseService
    {
        private readonly SupabaseClient _supabaseClient;
        private readonly IUsersRepository _userRepository;
        private readonly SupabaseSettingsOptions _supabaseSettingsOptions;
        private readonly ILogger<SupabaseService> _logger;

        public SupabaseService(SupabaseClient supabaseClient, IUsersRepository userRepository, IOptions<SupabaseSettingsOptions> supabaseSettingsOptions, ILogger<SupabaseService> logger)
        {
            _supabaseClient = supabaseClient;
            _userRepository = userRepository;
            _supabaseSettingsOptions = supabaseSettingsOptions.Value;
            _logger = logger;
        }

        public async Task<string> UploadAvatarAsync(IFormFile file, int userId)
        {
            if (file == null || file.Length == 0)
            {
                return string.Empty;
            }

            var fileExtension = Path.GetExtension(file.FileName);
            var filePath = $"{userId}/profile{fileExtension}";

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            await _supabaseClient.Storage
                .From(_supabaseSettingsOptions.Bucket)
                .Upload(fileBytes, filePath, new SupabaseStorageOptions
                {
                    Upsert = true,
                    ContentType = file.ContentType
                });
                _logger.LogInformation($"La imagen se ha guardado en el bucket: {_supabaseSettingsOptions.Bucket}");

            var baseAvatarUrl = _supabaseClient.Storage
                .From(_supabaseSettingsOptions.Bucket)
                .GetPublicUrl(filePath);

            var avatarUrl = $"{baseAvatarUrl}?t={DateTime.UtcNow.Ticks}";

            _logger.LogInformation($"Generated avatar URL: {avatarUrl}");
            
            var updatedUser = await _userRepository.SaveAvatarUrl(userId, avatarUrl);
            if (updatedUser is null)
            {
                throw new InvalidOperationException(string.Format(ResponseMessagesSupabase.SaveAvatarError, userId));
            }

            return avatarUrl;
        }
        public async Task<string> GetUrlAvatar(string userId)
        {
            return _supabaseClient.Storage
                .From(_supabaseSettingsOptions.Bucket)
                .GetPublicUrl(userId);
        }
    }
}