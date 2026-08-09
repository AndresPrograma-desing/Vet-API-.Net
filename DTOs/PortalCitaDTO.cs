using System;
using System.Text.Json.Serialization;

namespace DTOs;

public record PortalCitaDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("mascota_id")]
    public int MascotaId { get; set; }

    [JsonPropertyName("mascota_name")]
    public string MascotaNombre { get; set; } = string.Empty;

    [JsonPropertyName("fecha_cita")]
    public DateTime FechaCita { get; set; }

    [JsonPropertyName("hora_cita")]
    public string HoraCita { get; set; } = string.Empty;

    [JsonPropertyName("motivo")]
    public string Motivo { get; set; } = string.Empty;

    [JsonPropertyName("estado")]
    public string? Estado { get; set; }
}
