using System.Text.Json.Serialization;

namespace DTOs;

public record UpdatePetDTO
{
    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = null!;

    [JsonPropertyName("especie")]
    public string Especie { get; set; } = null!;

    [JsonPropertyName("raza")]
    public string? Raza { get; set; }

    [JsonPropertyName("color")]
    public string? Color { get; set; }

    [JsonPropertyName("sexo")]
    public string Sexo { get; set; } = null!;

    [JsonPropertyName("fecha_nacimiento")]
    public string? FechaNacimiento { get; set; }

    [JsonPropertyName("peso")]
    public decimal? Peso { get; set; }

    [JsonPropertyName("identeficacion_mascota")]
    public string? IdenteficacionMascota { get; set; }

    [JsonPropertyName("alergias")]
    public string? Alergias { get; set; }

    [JsonPropertyName("condiciones_medicas")]
    public string? CondicionesMedicas { get; set; }

    [JsonPropertyName("esterilizado")]
    public bool? Esterilizado { get; set; }
}
