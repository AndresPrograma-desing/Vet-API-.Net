using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DTOs;

public record GroqChatRequestDTO
{
    [JsonPropertyName("pregunta")]
    public string Pregunta { get; set; } = string.Empty;

    [JsonPropertyName("historial")]
    public List<GroqMessageDTO>? Historial { get; set; } = new();

    [JsonPropertyName("contactoId")]
    public string? ContactoId { get; set; }
}

public record GroqMessageDTO
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

public record GroqChatResponseDTO
{
    [JsonPropertyName("respuesta")]
    public string Respuesta { get; set; } = string.Empty;
}

public record GroqSessionTokenResponseDTO
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}
