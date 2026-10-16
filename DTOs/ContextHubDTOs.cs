using System.Text.Json.Serialization;

namespace DTOs;

//Describe: DTOs de request/response para la integración con el SDK externo ContextHub (login vía iframe + ticket de un solo uso).

public record ContextHubVerifyTicketRequest
{
    [JsonPropertyName("ticket")]
    public string Ticket { get; set; } = string.Empty;
}

public record ContextHubUserDTO
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}

public record ContextHubVerifyResponseDTO
{
    [JsonPropertyName("ok")]
    public bool Ok { get; set; }

    [JsonPropertyName("user")]
    public ContextHubUserDTO? User { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }
}

public record ContextHubCreateUserRequest
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}

public record ContextHubCreateUserResponseDTO
{
    [JsonPropertyName("ok")]
    public bool Ok { get; set; }

    [JsonPropertyName("user")]
    public ContextHubUserDTO? User { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }
}
