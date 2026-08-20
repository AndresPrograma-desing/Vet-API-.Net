using System;
using System.Text.Json.Serialization;

namespace DTOs;

public record RegisterPetVaccinationDTO
{
    [JsonPropertyName("mascota_id")]
    public int MascotaId { get; set; }

    [JsonPropertyName("vaccine_id")]
    public int VaccineId { get; set; }

    [JsonPropertyName("vaccine_batch_id")]
    public int VaccineBatchId { get; set; }

    [JsonPropertyName("consulta_id")]
    public int? ConsultaId { get; set; }

    [JsonPropertyName("doctor_id")]
    public int? DoctorId { get; set; }

    [JsonPropertyName("application_date")]
    public DateOnly? ApplicationDate { get; set; }

    [JsonPropertyName("weight_at_application")]
    public decimal? WeightAtApplication { get; set; }

    [JsonPropertyName("dose")]
    public string? Dose { get; set; }

    [JsonPropertyName("next_dose_date_override")]
    public DateOnly? NextDoseDateOverride { get; set; }

    [JsonPropertyName("clinical_observations")]
    public string? ClinicalObservations { get; set; }
}

public record PetVaccinationDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("mascota_id")]
    public int MascotaId { get; set; }

    [JsonPropertyName("vaccine_id")]
    public int VaccineId { get; set; }

    [JsonPropertyName("vaccine_name")]
    public string VaccineName { get; set; } = string.Empty;

    [JsonPropertyName("vaccine_batch_id")]
    public int? VaccineBatchId { get; set; }

    [JsonPropertyName("batch_number")]
    public string? BatchNumber { get; set; }

    [JsonPropertyName("consulta_id")]
    public int? ConsultaId { get; set; }

    [JsonPropertyName("doctor_id")]
    public int? DoctorId { get; set; }

    [JsonPropertyName("doctor_name")]
    public string? DoctorName { get; set; }

    [JsonPropertyName("application_date")]
    public DateOnly ApplicationDate { get; set; }

    [JsonPropertyName("weight_at_application")]
    public decimal? WeightAtApplication { get; set; }

    [JsonPropertyName("dose")]
    public string? Dose { get; set; }

    [JsonPropertyName("next_dose_date")]
    public DateOnly? NextDoseDate { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("postponement_reason")]
    public string? PostponementReason { get; set; }

    [JsonPropertyName("clinical_observations")]
    public string? ClinicalObservations { get; set; }

    [JsonPropertyName("detalle_factura_id")]
    public int? DetalleFacturaId { get; set; }
}

public record PostponeVaccinationDTO
{
    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;
}
