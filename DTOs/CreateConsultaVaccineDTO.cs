using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using vet_api_Net.Constants;

namespace DTOs;

public record CreateConsultaVaccineDTO
{
    [JsonPropertyName("vaccine_id")]
    [Range(1, int.MaxValue, ErrorMessage = ResponseMessagesDtos.ConsultaVaccine.VaccineIdInvalid)]
    public int VaccineId { get; set; }

    [JsonPropertyName("vaccine_batch_id")]
    [Range(1, int.MaxValue, ErrorMessage = ResponseMessagesDtos.ConsultaVaccine.VaccineBatchIdInvalid)]
    public int VaccineBatchId { get; set; }

    [JsonPropertyName("application_date")]
    public DateOnly? ApplicationDate { get; set; }

    [JsonPropertyName("weight_at_application")]
    [Range(0, double.MaxValue, ErrorMessage = ResponseMessagesDtos.Mascota.PesoNoNegativo)]
    public decimal? WeightAtApplication { get; set; }

    [JsonPropertyName("dose")]
    [StringLength(100, ErrorMessage = ResponseMessagesDtos.ConsultaVaccine.DoseMaxLength)]
    public string? Dose { get; set; }

    [JsonPropertyName("next_dose_date_override")]
    public DateOnly? NextDoseDateOverride { get; set; }

    [JsonPropertyName("clinical_observations")]
    [StringLength(500, ErrorMessage = ResponseMessagesDtos.ConsultaVaccine.ClinicalObservationsMaxLength)]
    public string? ClinicalObservations { get; set; }
}
