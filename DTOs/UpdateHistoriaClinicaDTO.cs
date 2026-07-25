using System.Text.Json.Serialization;

namespace DTOs;

public record UpdateHistoriaClinicaDTO
{
    [JsonPropertyName("notas_veterinario")]
    public string? NotasVeterinario { get; set; }
}
