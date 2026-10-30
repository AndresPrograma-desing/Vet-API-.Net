using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using vet_api_Net.Constants;

namespace DTOs;

public record ServicioDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("descripcion")]
    public string? Descripcion { get; set; }

    [JsonPropertyName("precio")]
    public decimal Precio { get; set; }

    [JsonPropertyName("activo")]
    public bool Activo { get; set; }

    [JsonPropertyName("creado")]
    public DateTime Creado { get; set; }
}

public record ServicioCreateDTO
{
    [JsonPropertyName("nombre")]
    [Required(ErrorMessage = ResponseMessagesDtos.Servicio.NombreRequired)]
    [StringLength(200, ErrorMessage = ResponseMessagesDtos.Servicio.NombreMaxLength)]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("descripcion")]
    [StringLength(500, ErrorMessage = ResponseMessagesDtos.Servicio.DescripcionMaxLength)]
    public string? Descripcion { get; set; }

    [JsonPropertyName("precio")]
    [Range(0, double.MaxValue, ErrorMessage = ResponseMessagesDtos.Servicio.PrecioNoNegativo)]
    public decimal Precio { get; set; }

    [JsonPropertyName("activo")]
    public bool Activo { get; set; } = true;
}

public record ServicioUpdateDTO
{
    [JsonPropertyName("nombre")]
    [StringLength(200, ErrorMessage = ResponseMessagesDtos.Servicio.NombreMaxLength)]
    public string? Nombre { get; set; }

    [JsonPropertyName("descripcion")]
    [StringLength(500, ErrorMessage = ResponseMessagesDtos.Servicio.DescripcionMaxLength)]
    public string? Descripcion { get; set; }

    [JsonPropertyName("precio")]
    [Range(0, double.MaxValue, ErrorMessage = ResponseMessagesDtos.Servicio.PrecioNoNegativo)]
    public decimal? Precio { get; set; }

    [JsonPropertyName("activo")]
    public bool? Activo { get; set; }
}

public record ServicioListResponseDTO
{
    [JsonPropertyName("items")]
    public List<ServicioDTO> Items { get; set; } = new();

    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }
}
