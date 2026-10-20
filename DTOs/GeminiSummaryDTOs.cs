using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DTOs;

public record GeminiPatientSummaryRequestDTO
{
    [JsonPropertyName("mascota_id")]
    public int MascotaId { get; set; }

    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("especie")]
    public string Especie { get; set; } = string.Empty;

    [JsonPropertyName("raza")]
    public string? Raza { get; set; }

    [JsonPropertyName("sexo")]
    public string Sexo { get; set; } = string.Empty;

    [JsonPropertyName("esterilizado")]
    public bool? Esterilizado { get; set; }

    [JsonPropertyName("fecha_nacimiento")]
    public DateOnly? FechaNacimiento { get; set; }

    [JsonPropertyName("peso")]
    public decimal? Peso { get; set; }

    [JsonPropertyName("alergias")]
    public string? Alergias { get; set; }

    [JsonPropertyName("condiciones_medicas")]
    public string? CondicionesMedicas { get; set; }

    [JsonPropertyName("consultas")]
    public List<GeminiConsultaResumenDTO> Consultas { get; set; } = new();

    [JsonPropertyName("vacunas")]
    public List<GeminiVacunaResumenDTO> Vacunas { get; set; } = new();

    [JsonPropertyName("resumen_anterior")]
    public string? ResumenAnterior { get; set; }
}

public record GeminiConsultaResumenDTO
{
    [JsonPropertyName("fecha_consulta")]
    public DateTime FechaConsulta { get; set; }

    [JsonPropertyName("sintomas")]
    public string? Sintomas { get; set; }

    [JsonPropertyName("diagnostico")]
    public string? Diagnostico { get; set; }

    [JsonPropertyName("tratamiento")]
    public string? Tratamiento { get; set; }

    [JsonPropertyName("peso_actual")]
    public decimal? PesoActual { get; set; }
}

public record GeminiVacunaResumenDTO
{
    [JsonPropertyName("nombre_vacuna")]
    public string NombreVacuna { get; set; } = string.Empty;

    [JsonPropertyName("fecha_vacunacion")]
    public DateOnly FechaVacunacion { get; set; }

    [JsonPropertyName("proxima_dosis")]
    public DateOnly? ProximaDosis { get; set; }
}

public record GeminiPatientSummaryResultDTO
{
    [JsonPropertyName("mascota_id")]
    public int MascotaId { get; set; }

    [JsonPropertyName("resumen_ia")]
    public string ResumenIa { get; set; } = string.Empty;

    [JsonPropertyName("alertas_riesgo_ia")]
    public string AlertasRiesgoIa { get; set; } = string.Empty;

    [JsonPropertyName("sugerencias_ia")]
    public string SugerenciasIa { get; set; } = string.Empty;
}
