using System;
using System.Text.Json.Serialization;

namespace DTOs;

public record MoneyTypesDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("money_name")]
    public string MoneyName { get; set; } = string.Empty;
    public string? TypeMoney {get; set;} 
    public decimal TasaBcv { get; set;}
}