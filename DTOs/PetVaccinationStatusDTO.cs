using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DTOs;

public record PetVaccinationStatusDTO
{
    [JsonPropertyName("vaccine_id")]
    public int VaccineId { get; set; }

    [JsonPropertyName("vaccine_name")]
    public string VaccineName { get; set; } = string.Empty;

    [JsonPropertyName("last_application_date")]
    public DateOnly? LastApplicationDate { get; set; }

    [JsonPropertyName("next_dose_date")]
    public DateOnly? NextDoseDate { get; set; }

    [JsonPropertyName("traffic_light")]
    public string TrafficLight { get; set; } = "Red";
}

public record PetVaccinationStatusResponseDTO
{
    [JsonPropertyName("mascota_id")]
    public int MascotaId { get; set; }

    [JsonPropertyName("items")]
    public List<PetVaccinationStatusDTO> Items { get; set; } = new();
}
