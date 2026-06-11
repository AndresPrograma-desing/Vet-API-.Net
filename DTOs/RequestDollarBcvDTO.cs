using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DTOs;

public class RequestDollarBcvDTO
{
    [JsonPropertyName("Money_type")]
    public string MoneyType { get; set; } = null!;

    [JsonPropertyName("BcvDollarPrice")]
    public decimal BcvDollar { get; set; }
}