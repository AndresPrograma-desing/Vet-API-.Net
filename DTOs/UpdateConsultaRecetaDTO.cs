using System;
using System.Text.Json.Serialization;

namespace DTOs;

public record UpdateConsultaRecetaDTO
{
    [JsonPropertyName("receta")]
    public string Receta { get; set; } = string.Empty;
}