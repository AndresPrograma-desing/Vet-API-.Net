using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using vet_api_Net.Infrastructure.Configuration;

namespace vet_api_Net.Interfaze.Security;

public class LoginSecurity(IOptions<SecurityOptions> security) : ILoginSecurity // C# 12
{
    private readonly SecurityOptions _security = security.Value;

    private class SecurityRecord
    {
        public int FailedAttempts { get; set; }
        public DateTime? LockoutEnd { get; set; }
    }
    
    private static readonly ConcurrentDictionary<string, SecurityRecord> _records = new();

    public Task<bool> IsBlockedAsync(string email)
    {
        if (_records.TryGetValue(email, out var record))
        {
            if (record.LockoutEnd.HasValue)
            {
                if (DateTime.UtcNow < record.LockoutEnd.Value)
                {
                    record.LockoutEnd = DateTime.UtcNow.AddSeconds(_security.LockoutDurationSeconds);

                    return Task.FromResult(true);
                }

                record.LockoutEnd = null;
                record.FailedAttempts = 0;
            }
        }

        return Task.FromResult(false);
    }

    public Task RegisterFailedAttemptAsync(string email)
    {
        var record = _records.GetOrAdd(email, _ => new SecurityRecord());
        record.FailedAttempts++;

        if (record.FailedAttempts >= _security.MaxAttempts)
        {
            record.LockoutEnd = DateTime.UtcNow.AddSeconds(_security.LockoutDurationSeconds);
        }

        return Task.CompletedTask;
    }

    public Task ResetAttemptsAsync(string email)
    {
        _records.TryRemove(email, out _);
        return Task.CompletedTask;
    }

    public Task<int> GetRemainingBlockTimeSecondsAsync(string email)
    {
        if (_records.TryGetValue(email, out var record) && record.LockoutEnd.HasValue)
        {
            var remaining = (record.LockoutEnd.Value - DateTime.UtcNow).TotalSeconds;
            return Task.FromResult(remaining > 0 ? (int)Math.Ceiling(remaining) : 0);
        }

        return Task.FromResult(0);
    }
}