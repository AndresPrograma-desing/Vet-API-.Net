using System;
using System.Text.Json.Serialization;

namespace DTOs;

public record IntegrationCredentialDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("provider")]
    public string Provider { get; set; } = string.Empty;

    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("daily_limit")]
    public int DailyLimit { get; set; }

    [JsonPropertyName("usage_count")]
    public int UsageCount { get; set; }

    [JsonPropertyName("usage_date")]
    public DateOnly UsageDate { get; set; }

    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; }
}

public record CreateIntegrationCredentialDTO
{
    [JsonPropertyName("provider")]
    public string Provider { get; set; } = string.Empty;

    [JsonPropertyName("secret_value")]
    public string SecretValue { get; set; } = string.Empty;

    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("daily_limit")]
    public int DailyLimit { get; set; } = 50;

    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; } = true;
}

public record UpdateIntegrationCredentialDTO
{
    [JsonPropertyName("secret_value")]
    public string? SecretValue { get; set; }

    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("daily_limit")]
    public int? DailyLimit { get; set; }

    [JsonPropertyName("is_active")]
    public bool? IsActive { get; set; }
}
