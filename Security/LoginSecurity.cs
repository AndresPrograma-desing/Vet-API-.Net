using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using vet_api_Net.Infrastructure.Configuration;

namespace vet_api_Net.Interfaze.Security;

public class LoginSecurity(IOptions<SecurityOptions> security) : ILoginSecurity
{
    private readonly SecurityOptions _security = security.Value;

    private sealed class SecurityRecord
    {
        public int FailedAttempts { get; set; }
        public DateTime? LockoutEnd { get; set; }
    }
    
    private static readonly ConcurrentDictionary<string, SecurityRecord> _records = new();

    public Task<bool> IsBlockedAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Task.FromResult(false);

        if (_records.TryGetValue(email, out var record))
        {
            lock (record)
            {
                if (record.LockoutEnd.HasValue)
                { 
                    if (DateTime.UtcNow < record.LockoutEnd.Value)
                    {
                        return Task.FromResult(true);
                    }
 
                    record.LockoutEnd = null;
                    record.FailedAttempts = 0;
                }
            }
        }

        return Task.FromResult(false);
    }

    public Task RegisterFailedAttemptAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Task.CompletedTask;

        _records.AddOrUpdate(
            email,
            _ =>
            {
                var newRecord = new SecurityRecord { FailedAttempts = 1 };
                if (newRecord.FailedAttempts >= _security.MaxAttempts)
                {
                    newRecord.LockoutEnd = DateTime.UtcNow.AddSeconds(_security.LockoutDurationSeconds);
                }
                return newRecord;
            },
            (_, existingRecord) =>
            {
                lock (existingRecord)
                {
                    existingRecord.FailedAttempts++;

                    if (existingRecord.FailedAttempts >= _security.MaxAttempts && !existingRecord.LockoutEnd.HasValue)
                    {
                        existingRecord.LockoutEnd = DateTime.UtcNow.AddSeconds(_security.LockoutDurationSeconds);
                    }

                    return existingRecord;
                }
            }
        );

        return Task.CompletedTask;
    }

    public Task ResetAttemptsAsync(string email)
    {
        if (!string.IsNullOrWhiteSpace(email))
        {
            _records.TryRemove(email, out _);
        }
        
        return Task.CompletedTask;
    }

    public Task<int> GetRemainingBlockTimeSecondsAsync(string email)
    {
        if (!string.IsNullOrWhiteSpace(email) && _records.TryGetValue(email, out var record))
        {
            lock (record)
            {
                if (record.LockoutEnd.HasValue)
                {
                    var remaining = (record.LockoutEnd.Value - DateTime.UtcNow).TotalSeconds;
                    return Task.FromResult(remaining > 0 ? (int)Math.Ceiling(remaining) : 0);
                }
            }
        }

        return Task.FromResult(0);
    }
}