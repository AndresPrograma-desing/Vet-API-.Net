using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DTOs;

public record HistoriaClinicaResponseDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("mascota_id")]
    public int MascotaId { get; set; }

    [JsonPropertyName("resumen")]
    public string? ResumenIa { get; set; }

    [JsonPropertyName("alertas_riesgo")]
    public string? AlertasRiesgoIa { get; set; }

    [JsonPropertyName("sugerencias")]
    public string? SugerenciasIa { get; set; }

    [JsonPropertyName("notas_veterinario")]
    public string? NotasVeterinario { get; set; }

    [JsonPropertyName("ultimo_analisis")]
    public DateTime? UltimoAnalisisIa { get; set; }

    [JsonPropertyName("creado")]
    public DateTime Creado { get; set; }

    [JsonPropertyName("actualizado")]
    public DateTime Actualizado { get; set; }

    [JsonPropertyName("mascota")]
    public HistoriaClinicaMascotaDTO Mascota { get; set; } = null!;

    [JsonPropertyName("consultas")]
    public List<HistoriaClinicaConsultaDTO> Consultas { get; set; } = new();

    [JsonPropertyName("vacunas")]
    public List<HistoriaClinicaVacunaDTO> Vacunas { get; set; } = new();

    [JsonPropertyName("versiones_anteriores")]
    public List<HistoriaClinicaVersionDTO> VersionesAnteriores { get; set; } = new();
}

public record HistoriaClinicaVersionDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("resumen")]
    public string? ResumenIa { get; set; }

    [JsonPropertyName("alertas_riesgo")]
    public string? AlertasRiesgoIa { get; set; }

    [JsonPropertyName("sugerencias")]
    public string? SugerenciasIa { get; set; }

    [JsonPropertyName("creado")]
    public DateTime Creado { get; set; }
}

public record HistoriaClinicaMascotaDTO
{
    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("especie")]
    public string Especie { get; set; } = string.Empty;

    [JsonPropertyName("raza")]
    public string? Raza { get; set; }

    [JsonPropertyName("sexo")]
    public string Sexo { get; set; } = string.Empty;

    [JsonPropertyName("fecha_nacimiento")]
    public DateOnly? FechaNacimiento { get; set; }

    [JsonPropertyName("peso")]
    public decimal? Peso { get; set; }

    [JsonPropertyName("alergias")]
    public string? Alergias { get; set; }

    [JsonPropertyName("condiciones_medicas")]
    public string? CondicionesMedicas { get; set; }
}

public record HistoriaClinicaConsultaDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("fecha_consulta")]
    public DateTime FechaConsulta { get; set; }

    [JsonPropertyName("sintomas")]
    public string Sintomas { get; set; } = string.Empty;

    [JsonPropertyName("diagnostico")]
    public string? Diagnostico { get; set; }

    [JsonPropertyName("tratamiento")]
    public string? Tratamiento { get; set; }

    [JsonPropertyName("receta")]
    public string? Receta { get; set; }

    [JsonPropertyName("observaciones")]
    public string? Observaciones { get; set; }

    [JsonPropertyName("doctor_nombre")]
    public string DoctorNombre { get; set; } = string.Empty;
}

public record HistoriaClinicaVacunaDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nombre_vacuna")]
    public string NombreVacuna { get; set; } = string.Empty;

    [JsonPropertyName("fecha_vacunacion")]
    public DateOnly FechaVacunacion { get; set; }

    [JsonPropertyName("proxima_dosis")]
    public DateOnly? ProximaDosis { get; set; }

    [JsonPropertyName("lote")]
    public string? Lote { get; set; }

    [JsonPropertyName("doctor_nombre")]
    public string? DoctorNombre { get; set; }

    [JsonPropertyName("nota")]
    public string? Nota { get; set; }
}
