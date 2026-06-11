using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DTOs;

public record UserPetsResponseDTO
{
    [JsonPropertyName("mascotas")]
    public List<MascotaResumenDTO> Mascotas { get; set; } = new List<MascotaResumenDTO>();
}
