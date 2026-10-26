using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DTOs;

public record ProcessEventsDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name_event")]
    public string NameEvent { get; set; } = string.Empty;

    [JsonPropertyName("type_event")]
    public string TypeEvent { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("error_message")]
    public string? ErrorMessage { get; set; }

    [JsonPropertyName("vinculation_data")]
    public string? VinculationData { get; set; }

    [JsonPropertyName("snapshot")]
    public string? Snapshot { get; set; }

    [JsonPropertyName("intents")]
    public int Intents { get; set; }

    [JsonPropertyName("is_resolved")]
    public bool IsResolved { get; set; }

    [JsonPropertyName("created")]
    public DateTime Created { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("resolved_at")]
    public DateTime? ResolvedAt { get; set; }
}

public record RegisterProcessEventDTO
{
    [JsonPropertyName("name_event")]
    public string NameEvent { get; set; } = string.Empty;

    [JsonPropertyName("type_event")]
    public string TypeEvent { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("error_message")]
    public string? ErrorMessage { get; set; }

    [JsonPropertyName("vinculation_data")]
    public string? VinculationData { get; set; }

    [JsonPropertyName("snapshot")]
    public string? Snapshot { get; set; }
}

public record ProcessEventsListResponseDTO
{
    [JsonPropertyName("items")]
    public List<ProcessEventsDTO> Items { get; set; } = new();

    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }
}
