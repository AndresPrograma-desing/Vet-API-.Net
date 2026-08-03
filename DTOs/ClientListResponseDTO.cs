using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DTOs;

public record ClientListResponseDTO
{
    [JsonPropertyName("items")]
    public List<ClientListWithMascotasDTO> Items { get; set; } = new();

    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }
}
