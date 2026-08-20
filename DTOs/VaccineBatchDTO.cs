using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DTOs;

public record VaccineBatchDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("vaccine_id")]
    public int VaccineId { get; set; }

    [JsonPropertyName("laboratory")]
    public string Laboratory { get; set; } = string.Empty;

    [JsonPropertyName("batch_number")]
    public string BatchNumber { get; set; } = string.Empty;

    [JsonPropertyName("expiration_date")]
    public DateOnly ExpirationDate { get; set; }

    [JsonPropertyName("quantity_in_stock")]
    public int QuantityInStock { get; set; }

    [JsonPropertyName("received_date")]
    public DateOnly ReceivedDate { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("is_expired")]
    public bool IsExpired { get; set; }
}

public record VaccineBatchCreateDTO
{
    [JsonPropertyName("laboratory")]
    public string Laboratory { get; set; } = string.Empty;

    [JsonPropertyName("batch_number")]
    public string BatchNumber { get; set; } = string.Empty;

    [JsonPropertyName("expiration_date")]
    public DateOnly ExpirationDate { get; set; }

    [JsonPropertyName("quantity_in_stock")]
    public int QuantityInStock { get; set; }

    [JsonPropertyName("received_date")]
    public DateOnly ReceivedDate { get; set; }
}

public record VaccineBatchListResponseDTO
{
    [JsonPropertyName("items")]
    public List<VaccineBatchDTO> Items { get; set; } = new();

    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }
}
