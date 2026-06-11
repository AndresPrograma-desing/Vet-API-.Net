using System;
using System.Text.Json.Serialization;

namespace DTOs;

public record GetNotificationDTO
{
    [JsonPropertyName("Title")]
    public string Titulo { get; set; } = string.Empty;
    [JsonPropertyName("Message")]
    public string Mensaje { get; set; } = string.Empty;
    [JsonPropertyName("Tipe")]
    public string Tipo { get; set; } = string.Empty;
    [JsonPropertyName("Priority")]
    public int Prioridad { get; set; } = 0;
    [JsonPropertyName("Destination")]
    public string Destino { get; set; } = string.Empty;
    [JsonPropertyName("Complete")]
    public bool Completada { get; set; } = false;
    [JsonPropertyName("Read")]
    public bool Leida { get; set; } = false;
    [JsonPropertyName("CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}