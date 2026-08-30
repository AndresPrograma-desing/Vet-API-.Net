using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using vet_api_Net.Constants;

namespace DTOs;

public record EspecieDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = string.Empty;
}

public record EspecieCreateDTO
{
    [JsonPropertyName("nombre")]
    [Required(ErrorMessage = ResponseMessagesEspecie.EspecieNombreRequired)]
    [StringLength(50, ErrorMessage = ResponseMessagesEspecie.EspecieMaxLength)]
    public string Nombre { get; set; } = string.Empty;
}
