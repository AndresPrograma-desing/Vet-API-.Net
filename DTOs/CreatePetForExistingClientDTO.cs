using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace DTOs;

public record CreatePetForExistingClientDTO
{
    [Required]
    public int ClienteId { get; set; }

    [Required]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public string Especie { get; set; } = string.Empty;

    public string? Raza { get; set; }
    public string? Color { get; set; }
    public string? Sexo { get; set; }
    public string? FechaNacimiento { get; set; }
    public decimal? Peso { get; set; }
    public string? IdenteficacionMascota { get; set; }
    public string? Alergias { get; set; }
    public string? CondicionesMedicas { get; set; }
    public bool Esterilizado { get; set; }
}

public record CreatePetForExistingClientResponseDTO
{
    public int MascotaId { get; set; }
    public int ClienteId { get; set; }
    public string MascotaNombre { get; set; } = string.Empty;
}