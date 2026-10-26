using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DTOs;

public record ServicioDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("descripcion")]
    public string? Descripcion { get; set; }

    [JsonPropertyName("precio")]
    public decimal Precio { get; set; }

    [JsonPropertyName("activo")]
    public bool Activo { get; set; }

    [JsonPropertyName("creado")]
    public DateTime Creado { get; set; }
}

public record ServicioCreateDTO
{
    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("descripcion")]
    public string? Descripcion { get; set; }

    [JsonPropertyName("precio")]
    public decimal Precio { get; set; }

    [JsonPropertyName("activo")]
    public bool Activo { get; set; } = true;
}

public record ServicioUpdateDTO
{
    [JsonPropertyName("nombre")]
    public string? Nombre { get; set; }

    [JsonPropertyName("descripcion")]
    public string? Descripcion { get; set; }

    [JsonPropertyName("precio")]
    public decimal? Precio { get; set; }

    [JsonPropertyName("activo")]
    public bool? Activo { get; set; }
}

public record ServicioListResponseDTO
{
    [JsonPropertyName("items")]
    public List<ServicioDTO> Items { get; set; } = new();

    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }
}
