using System;
using System.Text.Json.Serialization;

namespace DTOs;

public record WorkerConfigDTO
{
    [JsonPropertyName("worker_name")]
    public string WorkerName { get; set; } = string.Empty;

    [JsonPropertyName("is_enabled")]
    public bool IsEnabled { get; set; }

    [JsonPropertyName("interval_value")]
    public int IntervalValue { get; set; }

    [JsonPropertyName("interval_unit")]
    public string IntervalUnit { get; set; } = string.Empty;

    [JsonPropertyName("retention_value")]
    public int? RetentionValue { get; set; }

    [JsonPropertyName("retention_unit")]
    public string? RetentionUnit { get; set; }

    [JsonPropertyName("generate_enabled")]
    public bool? GenerateEnabled { get; set; }

    [JsonPropertyName("last_updated")]
    public DateTime LastUpdated { get; set; }
}
