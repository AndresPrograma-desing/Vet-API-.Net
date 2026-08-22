using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using vet_api_Net.Constants;

namespace DTOs;

public record FacturationItemDTO
{
    [JsonPropertyName("productos_consultas_id")]
    public int? ProductosConsultasId { get; init; }

    [JsonPropertyName("producto_id")]
    [Range(1, int.MaxValue, ErrorMessage = ResponseMessagesDtos.Factura.ProductoIdInvalid)]
    public int ProductoId { get; init; }

    [JsonPropertyName("codigo")]
    [Required(ErrorMessage = ResponseMessagesDtos.Factura.CodigoRequired)]
    [StringLength(50, ErrorMessage = ResponseMessagesDtos.Factura.CodigoMaxLength)]
    public string Codigo { get; init; } = null!;

    [JsonPropertyName("nombre")]
    [Required(ErrorMessage = ResponseMessagesDtos.Factura.ItemNombreRequired)]
    [StringLength(200, ErrorMessage = ResponseMessagesDtos.Factura.ItemNombreMaxLength)]
    public string Nombre { get; init; } = null!;

    [JsonPropertyName("descripcion")]
    [StringLength(500, ErrorMessage = ResponseMessagesDtos.Factura.ItemDescripcionMaxLength)]
    public string? Descripcion { get; init; }

    [JsonPropertyName("precio_unitario")]
    [Range(0, double.MaxValue, ErrorMessage = ResponseMessagesDtos.Factura.MontoNoNegativo)]
    public decimal PrecioUnitario { get; init; }

    [JsonPropertyName("cantidad")]
    [Range(1, int.MaxValue, ErrorMessage = ResponseMessagesDtos.Factura.CantidadPositiva)]
    public int Cantidad { get; init; }

    [JsonPropertyName("total")]
    [Range(0, double.MaxValue, ErrorMessage = ResponseMessagesDtos.Factura.MontoNoNegativo)]
    public decimal Total { get; init; }

    [JsonPropertyName("consulta_price")]
    [Range(0, double.MaxValue, ErrorMessage = ResponseMessagesDtos.Factura.MontoNoNegativo)]
    public decimal? ConsultaPrice { get; init; }
}

public record FacturationDTO
{
    [JsonPropertyName("factura_id")]
    public int? FacturaId { get; init; }

    [JsonPropertyName("numero_factura")]
    [Required(ErrorMessage = ResponseMessagesDtos.Factura.NumeroFacturaRequired)]
    [StringLength(50, ErrorMessage = ResponseMessagesDtos.Factura.NumeroFacturaMaxLength)]
    public string NumeroFactura { get; set; } = null!;

    [JsonPropertyName("cliente_id")]
    [Range(1, int.MaxValue, ErrorMessage = ResponseMessagesDtos.Factura.ClienteIdInvalid)]
    public int ClienteId { get; init; }

    [JsonPropertyName("cliente_nombre")]
    public string? ClienteNombre { get; init; }

    [JsonPropertyName("mascota_id")]
    [Range(1, int.MaxValue, ErrorMessage = ResponseMessagesDtos.Factura.MascotaIdInvalid)]
    public int? MascotaId { get; init; }

    [JsonPropertyName("mascota_nombre")]
    public string? MascotaNombre { get; init; }

    [JsonPropertyName("consulta_id")]
    public int? ConsultaId { get; init; }

    [JsonPropertyName("cita_id")]
    public int? CitaId { get; init; }

    [JsonPropertyName("doctor_id")]
    [Range(1, int.MaxValue, ErrorMessage = ResponseMessagesDtos.Factura.DoctorIdInvalid)]
    public int? DoctorId { get; init; }

    [JsonPropertyName("doctor_nombre")]
    public string? DoctorNombre { get; init; }

    [JsonPropertyName("fecha_emision")]
    public DateTime FechaEmision { get; init; }

    [JsonPropertyName("items")]
    [MinLength(1, ErrorMessage = ResponseMessagesDtos.Factura.ItemsRequired)]
    public List<FacturationItemDTO> Items { get; init; } = new List<FacturationItemDTO>();

    [JsonPropertyName("subtotal")]
    [Range(0, double.MaxValue, ErrorMessage = ResponseMessagesDtos.Factura.MontoNoNegativo)]
    public decimal Subtotal { get; init; }

    [JsonPropertyName("descuento")]
    [Range(0, double.MaxValue, ErrorMessage = ResponseMessagesDtos.Factura.MontoNoNegativo)]
    public decimal? Descuento { get; init; }

    [JsonPropertyName("total")]
    [Range(0, double.MaxValue, ErrorMessage = ResponseMessagesDtos.Factura.MontoNoNegativo)]
    public decimal Total { get; init; }

    [JsonPropertyName("metodo_pago")]
    [Required(ErrorMessage = ResponseMessagesDtos.Factura.MetodoPagoRequired)]
    [StringLength(50, ErrorMessage = ResponseMessagesDtos.Factura.MetodoPagoMaxLength)]
    public string MetodoPago { get; init; } = "pendiente";

    [JsonPropertyName("estado_pago")]
    [StringLength(50, ErrorMessage = ResponseMessagesDtos.Factura.EstadoPagoMaxLength)]
    public string? EstadoPago { get; init; }

    [JsonPropertyName("notas")]
    [StringLength(1000, ErrorMessage = ResponseMessagesDtos.Factura.NotasMaxLength)]
    public string? Notas { get; init; }

    [JsonPropertyName("creado")]
    public DateTime? Creado { get; init; }

    [JsonPropertyName("actualizado")]
    public DateTime? Actualizado { get; init; }

    [JsonPropertyName("url_docx")]
    public string? UrlDocx { get; set; }

    [JsonPropertyName("is_new")]
    public bool IsNew { get; init; } = false;
}

public record FacturationMoneyUse
{
    [JsonPropertyName("money_type")]
    public string MoneyType { get; init; } = null!;
}
