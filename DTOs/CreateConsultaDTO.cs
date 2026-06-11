using System;
using System.Text.Json.Serialization;

namespace DTOs;

public record CreateConsultaDTO
{
    [JsonPropertyName("cita_id")]
    public int CitaId { get; set; }

    [JsonPropertyName("mascota_id")]
    public int MascotaId { get; set; }

    [JsonPropertyName("doctor_id")]
    public int DoctorId { get; set; }

    [JsonPropertyName("fecha_consulta")]
    public DateTime? FechaConsulta { get; set; }

    [JsonPropertyName("peso_actual")]
    public decimal? PesoActual { get; set; }

    [JsonPropertyName("temperatura")]
    public decimal? Temperatura { get; set; }

    [JsonPropertyName("sintomas")]
    public string Sintomas { get; set; } = null!;

    [JsonPropertyName("diagnostico")]
    public string? Diagnostico { get; set; }

    [JsonPropertyName("tratamiento")]
    public string? Tratamiento { get; set; }

    [JsonPropertyName("receta")]
    public string? Receta { get; set; }

    [JsonPropertyName("observaciones")]
    public string? Observaciones { get; set; }
    [JsonPropertyName("consulta_price")]
    public decimal? ConsultaPrice { get; set; }

    [JsonPropertyName("productos")]
    public List<CreateConsultaProductoDTO>? Productos { get; set; }
}
