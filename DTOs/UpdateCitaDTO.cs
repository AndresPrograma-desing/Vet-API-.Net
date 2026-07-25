using System;
using System.Text.Json.Serialization;

namespace DTOs;

public record UpdateCitaDTO
{
    [JsonPropertyName("mascota_id")]
    public int MascotaId { get; set; }

    [JsonPropertyName("doctor_id")]
    public int DoctorId { get; set; }

    [JsonPropertyName("secretaria_id")]
    public int? SecretariaId { get; set; }

    [JsonPropertyName("fecha_cita")]
    public DateTime? FechaCita { get; set; }

    [JsonPropertyName("hora_cita")]
    public string? HoraCita { get; set; }

    [JsonPropertyName("motivo")]
    public string? Motivo { get; set; }

    [JsonPropertyName("tipo_cita")]
    public string? TipoCita { get; set; }

    [JsonPropertyName("estado")]
    public string? Estado { get; set; }

    [JsonPropertyName("notas")]
    public string? Notas { get; set; }
}
