using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using vet_api_Net.Constants;

namespace DTOs;

public record UpdateCitaDTO
{
    [JsonPropertyName("mascota_id")]
    [Range(1, int.MaxValue, ErrorMessage = ResponseMessagesCitas.InvalidMascotaId)]
    public int MascotaId { get; set; }

    [JsonPropertyName("doctor_id")]
    [Range(1, int.MaxValue, ErrorMessage = ResponseMessagesCitas.InvalidDoctorId)]
    public int DoctorId { get; set; }

    [JsonPropertyName("secretaria_id")]
    [Range(1, int.MaxValue, ErrorMessage = ResponseMessagesDtos.Cita.SecretariaIdInvalid)]
    public int? SecretariaId { get; set; }

    [JsonPropertyName("fecha_cita")]
    public DateTime? FechaCita { get; set; }

    [JsonPropertyName("hora_cita")]
    [RegularExpression(@"^([01]\d|2[0-3]):[0-5]\d(:[0-5]\d)?$", ErrorMessage = ResponseMessagesCitas.InvalidHoraCita)]
    public string? HoraCita { get; set; }

    [JsonPropertyName("motivo")]
    [StringLength(500, ErrorMessage = ResponseMessagesDtos.Cita.MotivoMaxLength)]
    public string? Motivo { get; set; }

    [JsonPropertyName("tipo_cita")]
    [StringLength(50, ErrorMessage = ResponseMessagesDtos.Cita.TipoCitaMaxLength)]
    public string? TipoCita { get; set; }

    [JsonPropertyName("estado")]
    [StringLength(50, ErrorMessage = ResponseMessagesDtos.Cita.EstadoMaxLength)]
    public string? Estado { get; set; }

    [JsonPropertyName("notas")]
    [StringLength(1000, ErrorMessage = ResponseMessagesDtos.Cita.NotasMaxLength)]
    public string? Notas { get; set; }
}
