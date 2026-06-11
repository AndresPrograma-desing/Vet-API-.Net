using System;
using System.Text.Json.Serialization;

namespace DTOs;

public record StatusCitaRequestDTO
{   
    [JsonPropertyName("estado")]
    public string Estado { get; set; } = null!;
}