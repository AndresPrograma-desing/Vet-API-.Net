using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using vet_api_Net.Constants;

namespace DTOs;

public record CreateConsultaDTO
{
    [JsonPropertyName("cita_id")]
    [Range(1, int.MaxValue, ErrorMessage = ResponseMessagesDtos.Consulta.CitaIdInvalid)]
    public int CitaId { get; set; }

    [JsonPropertyName("mascota_id")]
    [Range(1, int.MaxValue, ErrorMessage = ResponseMessagesDtos.Consulta.MascotaIdInvalid)]
    public int MascotaId { get; set; }

    [JsonPropertyName("doctor_id")]
    [Range(1, int.MaxValue, ErrorMessage = ResponseMessagesDtos.Consulta.DoctorIdInvalid)]
    public int DoctorId { get; set; }

    [JsonPropertyName("fecha_consulta")]
    public DateTime? FechaConsulta { get; set; }

    [JsonPropertyName("peso_actual")]
    [Range(0, double.MaxValue, ErrorMessage = ResponseMessagesDtos.Consulta.PesoActualNoNegativo)]
    public decimal? PesoActual { get; set; }

    [JsonPropertyName("temperatura")]
    [Range(0, 100, ErrorMessage = ResponseMessagesConsultas.TemperaturaInvalid)]
    public decimal? Temperatura { get; set; }

    [JsonPropertyName("sintomas")]
    [Required(ErrorMessage = ResponseMessagesDtos.Consulta.SintomasRequired)]
    [StringLength(1000, ErrorMessage = ResponseMessagesDtos.Consulta.SintomasMaxLength)]
    public string Sintomas { get; set; } = null!;

    [JsonPropertyName("diagnostico")]
    [StringLength(1000, ErrorMessage = ResponseMessagesDtos.Consulta.DiagnosticoMaxLength)]
    public string? Diagnostico { get; set; }

    [JsonPropertyName("tratamiento")]
    [StringLength(1000, ErrorMessage = ResponseMessagesDtos.Consulta.TratamientoMaxLength)]
    public string? Tratamiento { get; set; }

    [JsonPropertyName("receta")]
    [StringLength(1000, ErrorMessage = ResponseMessagesDtos.Consulta.RecetaMaxLength)]
    public string? Receta { get; set; }

    [JsonPropertyName("observaciones")]
    [StringLength(1000, ErrorMessage = ResponseMessagesDtos.Consulta.ObservacionesMaxLength)]
    public string? Observaciones { get; set; }
    [JsonPropertyName("consulta_price")]
    [Range(0, double.MaxValue, ErrorMessage = ResponseMessagesDtos.Consulta.ConsultaPriceNoNegativo)]
    public decimal? ConsultaPrice { get; set; }

    [JsonPropertyName("productos")]
    public List<CreateConsultaProductoDTO>? Productos { get; set; }
}
