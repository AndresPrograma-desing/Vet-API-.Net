using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DTOs;

public record ClientListWithMascotasDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = null!;

    [JsonPropertyName("apellido")]
    public string Apellido { get; set; } = null!;

    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;

    [JsonPropertyName("telefono")]
    public string? Telefono { get; set; }

    [JsonPropertyName("direccion")]
    public string? Direccion { get; set; }

    [JsonPropertyName("identificacion")]
    public string? Identificacion { get; set; }

    [JsonPropertyName("nota")]
    public string? Nota { get; set; }

    [JsonPropertyName("creado")]
    public DateTime Creado { get; set; }

    [JsonPropertyName("actualizado")]
    public DateTime Actualizado { get; set; }

    [JsonPropertyName("mascotas")]
    public List<MascotaResumenDTO> Mascotas { get; set; } = new List<MascotaResumenDTO>();
}
