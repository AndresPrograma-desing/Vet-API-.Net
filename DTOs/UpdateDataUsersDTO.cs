using System;
using System.Text.Json.Serialization;

namespace DTOs;

public class UpdateUserDTO
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }
}