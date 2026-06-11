using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DTOs;

public record NotificationCitaDTO
{
    [JsonPropertyName("fecha_cita")]
    public string FechaCita { get; set; } = string.Empty;
    
    [JsonPropertyName("hora_cita")]
    public string HoraCita { get; set; } = string.Empty;
    
    [JsonPropertyName("mascota_nombre")]
    public string MascotaNombre { get; set; } = string.Empty;
    
    [JsonPropertyName("cliente_nombre")]
    public string ClienteNombre { get; set; } = string.Empty;
}