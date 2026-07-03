using System.Text.Json.Serialization;

namespace DTOs;

public record UpdateBcvPriceRequest
{
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
}
