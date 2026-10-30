using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using vet_api_Net.Constants;
using vet_api_Net.Validation;

namespace DTOs;

public record CreatePetForExistingClientDTO
{
    [Required]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = ResponseMessagesDtos.Mascota.NombreRequired)]
    [StringLength(100, ErrorMessage = ResponseMessagesDtos.Mascota.NombreMaxLength)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = ResponseMessagesDtos.Mascota.EspecieRequired)]
    [StringLength(50, ErrorMessage = ResponseMessagesDtos.Mascota.EspecieMaxLength)]
    [NoAmbosEspecie(ErrorMessage = ResponseMessagesDtos.Mascota.EspecieNoAmbos)]
    public string Especie { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = ResponseMessagesDtos.Mascota.RazaMaxLength)]
    public string? Raza { get; set; }

    [StringLength(50, ErrorMessage = ResponseMessagesDtos.Mascota.ColorMaxLength)]
    public string? Color { get; set; }

    [StringLength(20, ErrorMessage = ResponseMessagesDtos.Mascota.SexoMaxLength)]
    public string? Sexo { get; set; }

    [StringLength(20, ErrorMessage = ResponseMessagesDtos.Mascota.FechaNacimientoMaxLength)]
    [NotFutureDate(ErrorMessage = ResponseMessagesDtos.Mascota.FechaNacimientoFutura)]
    public string? FechaNacimiento { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = ResponseMessagesDtos.Mascota.PesoNoNegativo)]
    public decimal? Peso { get; set; }

    [StringLength(50, ErrorMessage = ResponseMessagesDtos.Mascota.IdentificacionMaxLength)]
    public string? IdenteficacionMascota { get; set; }

    [StringLength(500, ErrorMessage = ResponseMessagesDtos.Mascota.AlergiasMaxLength)]
    public string? Alergias { get; set; }

    [StringLength(500, ErrorMessage = ResponseMessagesDtos.Mascota.CondicionesMedicasMaxLength)]
    public string? CondicionesMedicas { get; set; }

    public bool Esterilizado { get; set; }
}

public record CreatePetForExistingClientResponseDTO
{
    public int MascotaId { get; set; }
    public int ClienteId { get; set; }
    public string MascotaNombre { get; set; } = string.Empty;
}