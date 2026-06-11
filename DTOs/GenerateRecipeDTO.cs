using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DTOs;

public record GenerateRecipeDTO
{
  [JsonPropertyName("cita_id")]
  public int CitaId { get; set; }

  [JsonPropertyName("diagnostico")]
  public string Diagnostico { get; set; } = null!;
  [JsonPropertyName("tratamiento")]
  public string Tratamiento { get; set; } = null!;
  [JsonPropertyName("medicamentos")]
  public List<ProductDTO> Medicamentos { get; set; } = new List<ProductDTO>();


}
