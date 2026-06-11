using System;
using System.Text.Json.Serialization;

namespace DTOs
{
    public record WSMessageDTO
    {
        public string Numero { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public string NombreEmpresa { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        [JsonPropertyName("Url")]
        public string Url { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
    }

    public record WSInitSessionDTO
    {
        public string ClientId { get; set; } = string.Empty;
    }

    public record WSStatusResponseDTO
    {
        public string ClientId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Qr { get; set; } = string.Empty;
    }
    public record WSMessageDbDTO
    {
        public string? ClientId { get; set; }
        public string? ApiKey { get; set; }
        public string? Message { get; set; }
    }
}