using System.Text.Json.Serialization;

namespace DTOs;

public record UpdateFacturaEstadoDTO
{
    [JsonPropertyName("estado_pago")]
    public string EstadoPago { get; set; } = null!;

    [JsonPropertyName("metodo_pago")]
    public string? MetodoPago { get; set; }
} 
