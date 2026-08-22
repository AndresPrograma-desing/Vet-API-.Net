using System;
using System.ComponentModel.DataAnnotations;
using vet_api_Net.Constants;

namespace DTOs;

public record CreateMascotaDTO
{
    [Required(ErrorMessage = ResponseMessagesDtos.Mascota.NombreRequired)]
    [StringLength(100, ErrorMessage = ResponseMessagesDtos.Mascota.NombreMaxLength)]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = ResponseMessagesDtos.Mascota.EspecieRequired)]
    [StringLength(50, ErrorMessage = ResponseMessagesDtos.Mascota.EspecieMaxLength)]
    public string Especie { get; set; } = null!;

    [StringLength(50, ErrorMessage = ResponseMessagesDtos.Mascota.RazaMaxLength)]
    public string? Raza { get; set; }

    [StringLength(50, ErrorMessage = ResponseMessagesDtos.Mascota.ColorMaxLength)]
    public string? Color { get; set; }

    [Required(ErrorMessage = ResponseMessagesDtos.Mascota.SexoRequired)]
    [StringLength(20, ErrorMessage = ResponseMessagesDtos.Mascota.SexoMaxLength)]
    public string Sexo { get; set; } = null!;

    [StringLength(20, ErrorMessage = ResponseMessagesDtos.Mascota.FechaNacimientoMaxLength)]
    public string? FechaNacimiento { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = ResponseMessagesDtos.Mascota.PesoNoNegativo)]
    public decimal? Peso { get; set; }

    [StringLength(50, ErrorMessage = ResponseMessagesDtos.Mascota.IdentificacionMaxLength)]
    public string? IdenteficacionMascota { get; set; }

    [StringLength(500, ErrorMessage = ResponseMessagesDtos.Mascota.AlergiasMaxLength)]
    public string? Alergias { get; set; }

    [StringLength(500, ErrorMessage = ResponseMessagesDtos.Mascota.CondicionesMedicasMaxLength)]
    public string? CondicionesMedicas { get; set; }

    public bool? Esterilizado { get; set; }
}
