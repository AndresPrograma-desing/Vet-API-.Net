using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DTOs;

public record MovimientoInventarioDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("producto_id")]
    public int ProductoId { get; set; }

    [JsonPropertyName("producto_nombre")]
    public string ProductoNombre { get; set; } = string.Empty;

    [JsonPropertyName("tipo_movimiento")]
    public string TipoMovimiento { get; set; } = string.Empty;

    [JsonPropertyName("cantidad")]
    public int Cantidad { get; set; }

    [JsonPropertyName("stock_anterior")]
    public int StockAnterior { get; set; }

    [JsonPropertyName("stock_nuevo")]
    public int StockNuevo { get; set; }

    [JsonPropertyName("motivo")]
    public string Motivo { get; set; } = string.Empty;

    [JsonPropertyName("referencia_id")]
    public int? ReferenciaId { get; set; }

    [JsonPropertyName("usuario_id")]
    public int UsuarioId { get; set; }

    [JsonPropertyName("usuario_nombre")]
    public string? UsuarioNombre { get; set; }

    [JsonPropertyName("fecha_movimiento")]
    public DateTime FechaMovimiento { get; set; }
}

public record MovimientoInventarioListResponseDTO
{
    [JsonPropertyName("items")]
    public List<MovimientoInventarioDTO> Items { get; set; } = new();

    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }
}

public record ProductoStockBajoDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("codigo")]
    public string Codigo { get; set; } = string.Empty;

    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("stock")]
    public int? Stock { get; set; }

    [JsonPropertyName("stock_minimo")]
    public int? StockMinimo { get; set; }
}
