using System;
using System.Text.Json.Serialization;


namespace DTOs;

public record DeleteCitaDTO
{
     [JsonPropertyName("id")]
    public int Id { get; set; }
}