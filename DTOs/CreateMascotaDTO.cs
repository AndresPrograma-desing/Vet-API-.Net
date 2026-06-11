using System;

namespace DTOs;

public record CreateMascotaDTO
{
    public string Nombre { get; set; } = null!;
    public string Especie { get; set; } = null!;
    public string? Raza { get; set; }
    public string? Color { get; set; }
    public string Sexo { get; set; } = null!;
    public string? FechaNacimiento { get; set; }
    public decimal? Peso { get; set; }
    public string? IdenteficacionMascota { get; set; }
    public string? Alergias { get; set; }
    public string? CondicionesMedicas { get; set; }
    public bool? Esterilizado { get; set; }
}
