using System;
using System.Text.Json.Serialization;

namespace DTOs;

public record FacturaSettingDTO
{
    [JsonPropertyName("daysf")]
    public int Days { get; set; } = 30;
    [JsonPropertyName("isEnabledf")]
    public bool IsEnabled { get; set; } = true;
    [JsonPropertyName("generateEnabledf")]
    public bool GenerateEnabled { get; set; } = true;
    [JsonPropertyName("lastUpdatedf")]
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}