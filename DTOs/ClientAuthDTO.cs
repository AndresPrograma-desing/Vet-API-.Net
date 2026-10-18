using System.Text.Json.Serialization;

namespace DTOs;

public record ClientAccessRequestDTO
{
    [JsonPropertyName("identifier")]
    public string Identifier { get; set; } = string.Empty;
}

public record ClientVerifyAccessDTO
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
}

public record ClientLoginResponseDTO
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("client")]
    public ClientProfileDTO Client { get; set; } = null!;
}

public record ClientProfileDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("apellido")]
    public string Apellido { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }
}
