using System.Threading.Tasks;

namespace vet_api_Net.Interfaze.Services;

public interface IPasswordRecoveryService
{
    Task<bool> RequestRecoveryCodeAsync(string email);
    Task<bool> VerifyCodeAndSendPasswordAsync(string code);
}
