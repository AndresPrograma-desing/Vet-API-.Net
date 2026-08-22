using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using vet_api_Net.Constants;

namespace DTOs;

public record StatusCitaRequestDTO
{
    [JsonPropertyName("estado")]
    [Required(ErrorMessage = ResponseMessagesDtos.Cita.EstadoRequired)]
    [StringLength(50, ErrorMessage = ResponseMessagesDtos.Cita.EstadoMaxLength)]
    public string Estado { get; set; } = null!;
}