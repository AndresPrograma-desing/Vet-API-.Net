using System.Text.Json.Serialization;

namespace DTOs;

public record ResumenClinicoIAResponseDTO
{
    [JsonPropertyName("resumen_ia")]
    public string ResumenIa { get; set; } = string.Empty;

    [JsonPropertyName("alertas_riesgo_ia")]
    public string AlertasRiesgoIa { get; set; } = string.Empty;

    [JsonPropertyName("sugerencias_ia")]
    public string SugerenciasIa { get; set; } = string.Empty;
}
