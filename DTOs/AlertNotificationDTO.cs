using System;
using System.Text.Json.Serialization;

namespace vet_api_Net.DTOs;

public record AlertNotificationDTO
{
    [JsonPropertyName("id")
    ]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Titulo { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Mensaje { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string? Tipo { get; set; }

    [JsonPropertyName("priority")]
    public string Prioridad { get; set; } = "0";

    [JsonPropertyName("destination")]
    public string Destino { get; set; } = string.Empty;

    [JsonPropertyName("origin")]
    public string Origen { get; set; } = string.Empty;
    [JsonPropertyName("reference")]
    public string Referencia { get; set; } = string.Empty;

    [JsonPropertyName("completed")]
    public bool Completada { get; set; }

    [JsonPropertyName("is_read")]
    public bool Leida { get; set; }
    [JsonPropertyName("read_at")]
    public DateTime? FechaLectura { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }
}