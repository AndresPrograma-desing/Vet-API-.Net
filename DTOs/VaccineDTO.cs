using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DTOs;

public record VaccineDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("species")]
    public string Species { get; set; } = string.Empty;

    [JsonPropertyName("minimum_age_weeks")]
    public int MinimumAgeWeeks { get; set; }

    [JsonPropertyName("booster_frequency_value")]
    public int BoosterFrequencyValue { get; set; }

    [JsonPropertyName("booster_frequency_unit")]
    public string BoosterFrequencyUnit { get; set; } = "days";

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("producto_id")]
    public int? ProductoId { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("scheme_stages")]
    public List<VaccineSchemeStageDTO> SchemeStages { get; set; } = new();
}

public record VaccineCreateDTO
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("species")]
    public string Species { get; set; } = string.Empty;

    [JsonPropertyName("minimum_age_weeks")]
    public int MinimumAgeWeeks { get; set; }

    [JsonPropertyName("booster_frequency_value")]
    public int BoosterFrequencyValue { get; set; }

    [JsonPropertyName("booster_frequency_unit")]
    public string BoosterFrequencyUnit { get; set; } = "days";

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("producto_id")]
    public int? ProductoId { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; } = true;
}

public record VaccineListResponseDTO
{
    [JsonPropertyName("items")]
    public List<VaccineDTO> Items { get; set; } = new();

    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }
}

public record VaccineSchemeStageDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("vaccine_id")]
    public int VaccineId { get; set; }

    [JsonPropertyName("stage_type")]
    public string StageType { get; set; } = string.Empty;

    [JsonPropertyName("dose_number")]
    public int DoseNumber { get; set; }

    [JsonPropertyName("interval_days_from_previous")]
    public int IntervalDaysFromPrevious { get; set; }

    [JsonPropertyName("minimum_age_weeks_override")]
    public int? MinimumAgeWeeksOverride { get; set; }
}

public record VaccineSchemeStageCreateDTO
{
    [JsonPropertyName("stage_type")]
    public string StageType { get; set; } = string.Empty;

    [JsonPropertyName("dose_number")]
    public int DoseNumber { get; set; }

    [JsonPropertyName("interval_days_from_previous")]
    public int IntervalDaysFromPrevious { get; set; }

    [JsonPropertyName("minimum_age_weeks_override")]
    public int? MinimumAgeWeeksOverride { get; set; }
}
