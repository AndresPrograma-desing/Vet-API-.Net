using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DTOs;

public record MascotaListResponseDTO
{
    [JsonPropertyName("items")]
    public List<MascotaResumenDTO> Items { get; set; } = new();

    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }
}
