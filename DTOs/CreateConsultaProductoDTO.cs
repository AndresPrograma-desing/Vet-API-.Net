using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using vet_api_Net.Constants;

namespace DTOs;

public record CreateConsultaProductoDTO
{
    [JsonPropertyName("producto_id")]
    [Range(1, int.MaxValue, ErrorMessage = ResponseMessagesDtos.ConsultaProducto.ProductoIdInvalid)]
    public int ProductoId { get; set; }

    [JsonPropertyName("cantidad")]
    [Range(1, int.MaxValue, ErrorMessage = ResponseMessagesDtos.ConsultaProducto.CantidadPositiva)]
    public int Cantidad { get; set; } = 1;

    [JsonPropertyName("precio_unitario")]
    [Range(0, double.MaxValue, ErrorMessage = ResponseMessagesDtos.ConsultaProducto.PrecioUnitarioNoNegativo)]
    public decimal? PrecioUnitario { get; set; }

    [JsonPropertyName("dosis")]
    [StringLength(100, ErrorMessage = ResponseMessagesDtos.ConsultaProducto.DosisMaxLength)]
    public string? Dosis { get; set; }

    [JsonPropertyName("via_administracion")]
    [StringLength(100, ErrorMessage = ResponseMessagesDtos.ConsultaProducto.ViaAdministracionMaxLength)]
    public string? ViaAdministracion { get; set; }

    [JsonPropertyName("frecuencia")]
    [StringLength(100, ErrorMessage = ResponseMessagesDtos.ConsultaProducto.FrecuenciaMaxLength)]
    public string? Frecuencia { get; set; }

    [JsonPropertyName("duracion")]
    [StringLength(100, ErrorMessage = ResponseMessagesDtos.ConsultaProducto.DuracionMaxLength)]
    public string? Duracion { get; set; }

    [JsonPropertyName("instrucciones")]
    [StringLength(500, ErrorMessage = ResponseMessagesDtos.ConsultaProducto.InstruccionesMaxLength)]
    public string? Instrucciones { get; set; }

    [JsonPropertyName("aplicado_por")]
    [Range(1, int.MaxValue, ErrorMessage = ResponseMessagesDtos.ConsultaProducto.AplicadoPorInvalid)]
    public int? AplicadoPor { get; set; }
}
