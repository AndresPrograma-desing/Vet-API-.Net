using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DTOs;

public record RequestPasswordResetDTO
{
    [Required]
    [EmailAddress]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}

public record AssignNewPasswordDTO
{
    [Required]
    [EmailAddress]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    [JsonPropertyName("new_password")]
    public string NewPassword { get; set; } = string.Empty;
}

public record ResetStatusResponseDTO
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("expiracion")]
    public DateTime? Expiracion { get; set; }

    [JsonPropertyName("expires_in_seconds")]
    public double? ExpiresInSeconds { get; set; }
}
