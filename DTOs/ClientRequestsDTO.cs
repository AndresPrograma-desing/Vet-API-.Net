using System;
using System.Text.Json.Serialization;

namespace DTOs;

public record ClientCancelCitaDTO
{
    [JsonPropertyName("motivo")]
    public string Motivo { get; set; } = string.Empty;
}

public record ClientConfirmCitaDTO
{
    [JsonPropertyName("mensaje")]
    public string? Mensaje { get; set; }
}

public record ClientRequestCitaDTO
{
    [JsonPropertyName("mascota_id")]
    public int MascotaId { get; set; }

    [JsonPropertyName("motivo")]
    public string Motivo { get; set; } = string.Empty;

    [JsonPropertyName("fecha_preferida")]
    public DateTime? FechaPreferida { get; set; }

    [JsonPropertyName("hora_preferida")]
    public string? HoraPreferida { get; set; }
}

public record ClientRequestRecetaRenewalDTO
{
    [JsonPropertyName("nota")]
    public string Nota { get; set; } = string.Empty;
}
