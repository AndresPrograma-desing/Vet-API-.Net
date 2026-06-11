using System;
using System.Text.Json.Serialization;

namespace DTOs;

public record CategoryProductsDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = null!;

    [JsonPropertyName("descripcion")]
    public string? Descripcion { get; set; }
}