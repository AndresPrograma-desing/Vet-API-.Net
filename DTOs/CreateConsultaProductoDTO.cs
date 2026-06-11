using System.Text.Json.Serialization;

namespace DTOs;

public record CreateConsultaProductoDTO
{
    [JsonPropertyName("producto_id")]
    public int ProductoId { get; set; }

    [JsonPropertyName("cantidad")]
    public int Cantidad { get; set; } = 1;

    [JsonPropertyName("precio_unitario")]
    public decimal? PrecioUnitario { get; set; }

    [JsonPropertyName("dosis")]
    public string? Dosis { get; set; }

    [JsonPropertyName("via_administracion")]
    public string? ViaAdministracion { get; set; }

    [JsonPropertyName("frecuencia")]
    public string? Frecuencia { get; set; }

    [JsonPropertyName("duracion")]
    public string? Duracion { get; set; }

    [JsonPropertyName("instrucciones")]
    public string? Instrucciones { get; set; }

    [JsonPropertyName("aplicado_por")]
    public int? AplicadoPor { get; set; }
}
