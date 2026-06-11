using System;
using System.Text.Json.Serialization;

namespace DTOs;

public record ProductDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("codigo")]
    public string Codigo { get; set; } = null!;

    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = null!;

    [JsonPropertyName("descripcion")]
    public string? Descripcion { get; set; }

    [JsonPropertyName("categoria_id")]
    public int? CategoriaId { get; set; }

    [JsonPropertyName("tipo")]
    public string? Tipo { get; set; }

    [JsonPropertyName("precio")]
    public decimal Precio { get; set; }

    [JsonPropertyName("precio_venta")]
    public decimal PrecioVenta { get; set; }

    [JsonPropertyName("stock")]
    public int? Stock { get; set; }

    [JsonPropertyName("stock_minimo")]
    public int? StockMinimo { get; set; }
    [JsonPropertyName("unidad_medida")]
    public string? UnidadMedida { get; set; }


    [JsonPropertyName("proveedor")]
    public string? Proveedor { get; set; }
}
