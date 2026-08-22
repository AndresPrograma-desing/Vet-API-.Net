using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using vet_api_Net.Constants;

namespace DTOs;

public record ClientRequestDTO
{
    [JsonPropertyName("nombre")]
    [Required(ErrorMessage = ResponseMessagesDtos.Cliente.NombreRequired)]
    [StringLength(100, ErrorMessage = ResponseMessagesDtos.Cliente.NombreMaxLength)]
    public string Nombre { get; set; } = null!;

    [JsonPropertyName("apellido")]
    [Required(ErrorMessage = ResponseMessagesDtos.Cliente.ApellidoRequired)]
    [StringLength(100, ErrorMessage = ResponseMessagesDtos.Cliente.ApellidoMaxLength)]
    public string Apellido { get; set; } = null!;

    [JsonPropertyName("email")]
    [Required(ErrorMessage = ResponseMessagesDtos.Cliente.EmailRequired)]
    [EmailAddress(ErrorMessage = ResponseMessagesDtos.Cliente.InvalidEmailFormat)]
    public string Email { get; set; } = null!;

    [JsonPropertyName("telefono")]
    [StringLength(20, ErrorMessage = ResponseMessagesDtos.Cliente.TelefonoMaxLength)]
    public string? Telefono { get; set; }

    [JsonPropertyName("direccion")]
    [StringLength(300, ErrorMessage = ResponseMessagesDtos.Cliente.DireccionMaxLength)]
    public string? Direccion { get; set; }

    [JsonPropertyName("identificacion")]
    [StringLength(20, ErrorMessage = ResponseMessagesDtos.Cliente.IdentificacionMaxLength)]
    public string? Identificacion { get; set; }

    [JsonPropertyName("nota")]
    [StringLength(500, ErrorMessage = ResponseMessagesDtos.Cliente.NotaMaxLength)]
    public string? Nota { get; set; }
}