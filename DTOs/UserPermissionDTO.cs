using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DTOs;

public record UserPermissionsResponseDTO
{
    [JsonPropertyName("user_id")]
    public int UserId { get; set; }

    [JsonPropertyName("permissions")]
    public List<string>? Permissions { get; set; }
}

public record UpdateUserPermissionsDTO
{
    [JsonPropertyName("permissions")]
    public List<string> Permissions { get; set; } = new();
}
