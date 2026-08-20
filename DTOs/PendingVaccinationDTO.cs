using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DTOs;

public record PendingVaccinationDTO
{
    [JsonPropertyName("pet_vaccination_id")]
    public int PetVaccinationId { get; set; }

    [JsonPropertyName("mascota_id")]
    public int MascotaId { get; set; }

    [JsonPropertyName("mascota_nombre")]
    public string MascotaNombre { get; set; } = string.Empty;

    [JsonPropertyName("cliente_nombre")]
    public string ClienteNombre { get; set; } = string.Empty;

    [JsonPropertyName("cliente_telefono")]
    public string? ClienteTelefono { get; set; }

    [JsonPropertyName("vaccine_name")]
    public string VaccineName { get; set; } = string.Empty;

    [JsonPropertyName("next_dose_date")]
    public DateOnly NextDoseDate { get; set; }
}

public record PendingVaccinationListResponseDTO
{
    [JsonPropertyName("items")]
    public List<PendingVaccinationDTO> Items { get; set; } = new();

    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }
}
