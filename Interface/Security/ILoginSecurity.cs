using System;

namespace vet_api_Net.Interfaze.Security;

public interface ILoginSecurity
{
    Task<bool> IsBlockedAsync(string email);
    Task RegisterFailedAttemptAsync(string email);
    Task ResetAttemptsAsync(string email);
    Task<int> GetRemainingBlockTimeSecondsAsync(string email);
}