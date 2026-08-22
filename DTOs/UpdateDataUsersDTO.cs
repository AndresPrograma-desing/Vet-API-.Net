using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using vet_api_Net.Constants;

namespace DTOs;

public class UpdateUserDTO
{
    [JsonPropertyName("name")]
    [StringLength(100, ErrorMessage = ResponseMessagesDtos.Usuario.NombreMaxLength)]
    public string? Name { get; set; }

    [JsonPropertyName("lastName")]
    [StringLength(100, ErrorMessage = ResponseMessagesDtos.Usuario.ApellidoMaxLength)]
    public string? LastName { get; set; }

    [JsonPropertyName("email")]
    [EmailAddress(ErrorMessage = ResponseMessagesDtos.Usuario.InvalidEmailFormat)]
    public string? Email { get; set; }

    [JsonPropertyName("phone")]
    [StringLength(20, ErrorMessage = ResponseMessagesDtos.Usuario.TelefonoMaxLength)]
    public string? Phone { get; set; }
}