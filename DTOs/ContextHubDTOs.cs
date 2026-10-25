using System.Collections.Generic;
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

public record ContextHubMailAttachment
{
    [JsonPropertyName("filename")]
    public string Filename { get; set; } = string.Empty;

    [JsonPropertyName("contentType")]
    public string ContentType { get; set; } = string.Empty;

    [JsonPropertyName("contentBase64")]
    public string ContentBase64 { get; set; } = string.Empty;
}

public record ContextHubSendMailRequest
{
    [JsonPropertyName("to")]
    public string? To { get; set; }

    [JsonPropertyName("subject")]
    public string Subject { get; set; } = string.Empty;

    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("html")]
    public string? Html { get; set; }

    [JsonPropertyName("fromName")]
    public string? FromName { get; set; }

    [JsonPropertyName("replyTo")]
    public string? ReplyTo { get; set; }

    [JsonPropertyName("attachments")]
    public List<ContextHubMailAttachment>? Attachments { get; set; }
}

public record ContextHubSendMailResponseDTO
{
    [JsonPropertyName("accepted")]
    public bool Accepted { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }
}

public record ContextHubMailErrorDetail
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }
}

public record ContextHubMailErrorResponseDTO
{
    [JsonPropertyName("error")]
    public ContextHubMailErrorDetail? Error { get; set; }
}
