using System.Text.Json.Serialization;

namespace DTOs;

public record IaConocimientoSaveDTO
{
    [JsonPropertyName("categoria")]
    public string Categoria { get; set; } = string.Empty;

    [JsonPropertyName("reglas_respuesta")]
    public string ReglasRespuesta { get; set; } = string.Empty;

    [JsonPropertyName("base_conocimiento")]
    public string BaseConocimiento { get; set; } = string.Empty;
}

public record IaConocimientoResponseDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("categoria")]
    public string Categoria { get; set; } = string.Empty;

    [JsonPropertyName("reglas_respuesta")]
    public string ReglasRespuesta { get; set; } = string.Empty;

    [JsonPropertyName("base_conocimiento")]
    public string BaseConocimiento { get; set; } = string.Empty;
}
