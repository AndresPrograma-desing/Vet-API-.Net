using System.Text.Json.Serialization;

namespace DTOs;

public record WorkingHoursDTO
{
    [JsonPropertyName("start_time")]
    public string StartTime { get; set; } = string.Empty;

    [JsonPropertyName("end_time")]
    public string EndTime { get; set; } = string.Empty;
}
