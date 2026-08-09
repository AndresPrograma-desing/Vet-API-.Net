using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DTOs;

public record CitasListResponseDTO
{
    [JsonPropertyName("items")]
    public List<CitasRequestDTO> Items { get; set; } = new();

    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }
}
