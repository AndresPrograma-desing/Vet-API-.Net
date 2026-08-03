using System;
using System.Text.Json.Serialization;

namespace DTOs;

public record LogsSistemaResponseDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("usuario_id")]
    public int? UsuarioId { get; set; }

    [JsonPropertyName("usuario_nombre")]
    public string? UsuarioNombre { get; set; }

    [JsonPropertyName("accion")]
    public string Accion { get; set; } = string.Empty;

    [JsonPropertyName("tabla_afectada")]
    public string? TablaAfectada { get; set; }

    [JsonPropertyName("registro_id")]
    public int? RegistroId { get; set; }

    [JsonPropertyName("datos_previos")]
    public string? DatosPrevios { get; set; }

    [JsonPropertyName("datos_nuevos")]
    public string? DatosNuevos { get; set; }

    [JsonPropertyName("ip_address")]
    public string? IpAddress { get; set; }

    [JsonPropertyName("user_agent")]
    public string? UserAgent { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
}
