using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DTOs;

public record HistorialPrecioDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("producto_id")]
    public int ProductoId { get; set; }

    [JsonPropertyName("producto_nombre")]
    public string ProductoNombre { get; set; } = string.Empty;

    [JsonPropertyName("precio_anterior")]
    public decimal PrecioAnterior { get; set; }

    [JsonPropertyName("precio_nuevo")]
    public decimal PrecioNuevo { get; set; }

    [JsonPropertyName("usuario_id")]
    public int UsuarioId { get; set; }

    [JsonPropertyName("usuario_nombre")]
    public string? UsuarioNombre { get; set; }

    [JsonPropertyName("motivo")]
    public string? Motivo { get; set; }

    [JsonPropertyName("fecha_cambio")]
    public DateTime FechaCambio { get; set; }
}

public record HistorialPrecioListResponseDTO
{
    [JsonPropertyName("items")]
    public List<HistorialPrecioDTO> Items { get; set; } = new();

    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }
}
