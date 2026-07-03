using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DTOs;

public class RequestDollarBcvDTO
{
    [JsonPropertyName("Money_type")]
    public string MoneyType { get; set; } = null!;

    [JsonPropertyName("BcvDollarPrice")]
    public string? BcvDollar { get; set; }

    [JsonPropertyName("Message")]
    public string? Message { get; set; }
}