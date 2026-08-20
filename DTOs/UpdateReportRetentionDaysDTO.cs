using System.Text.Json.Serialization;

namespace DTOs;

public record UpdateReportRetentionDaysDTO
{
    [JsonPropertyName("days")]
    public int Days { get; set; }
}
