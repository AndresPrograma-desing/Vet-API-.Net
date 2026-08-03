using System.Threading.Tasks;

namespace vet_api_Net.Interfaze.Services;

public interface IPasswordRecoveryService
{
    Task<bool> RequestRecoveryCodeAsync(string identifier);
    Task<bool> VerifyCodeAndSendPasswordAsync(string code);
}
