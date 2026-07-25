using System.Text.Json.Serialization;

namespace DTOs;

public record UpdateClientDTO
{
    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = null!;

    [JsonPropertyName("apellido")]
    public string Apellido { get; set; } = null!;

    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;

    [JsonPropertyName("telefono")]
    public string? Telefono { get; set; }

    [JsonPropertyName("direccion")]
    public string? Direccion { get; set; }

    [JsonPropertyName("identificacion")]
    public string? Identificacion { get; set; }

    [JsonPropertyName("nota")]
    public string? Nota { get; set; }
}
