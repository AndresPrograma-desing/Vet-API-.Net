using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using vet_api_Net.Constants;

namespace DTOs;

public record UpdateFacturaEstadoDTO
{
    [JsonPropertyName("estado_pago")]
    [Required(ErrorMessage = ResponseMessagesFacturaController.StatusPagoIsRequired)]
    [StringLength(50, ErrorMessage = ResponseMessagesDtos.Factura.EstadoPagoMaxLength)]
    public string EstadoPago { get; set; } = null!;

    [JsonPropertyName("metodo_pago")]
    [StringLength(50, ErrorMessage = ResponseMessagesDtos.Factura.MetodoPagoMaxLength)]
    public string? MetodoPago { get; set; }
}
