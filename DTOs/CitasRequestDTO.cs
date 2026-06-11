using System;
using System.Text.Json.Serialization;

namespace DTOs;

public record CitasRequestDTO
{
	[JsonPropertyName("id")]
	public int Id { get; set; }

	[JsonPropertyName("mascota_id")]
	public int MascotaId { get; set; }

	[JsonPropertyName("doctor_id")]
	public int DoctorId { get; set; }

	[JsonPropertyName("secretaria_id")]
	[JsonIgnore(Condition = JsonIgnoreCondition.Never)]
	public int? SecretariaId { get; set; }

	[JsonPropertyName("fecha_cita")]
	public string FechaCita { get; set; } = null!;

	[JsonPropertyName("hora_cita")]
	public string HoraCita { get; set; } = null!;

	[JsonPropertyName("motivo")]
	public string Motivo { get; set; } = null!;

	[JsonPropertyName("tipo_cita")]
	public string TipoCita { get; set; } = string.Empty;

	[JsonPropertyName("estado")]
	public string Estado { get; set; } = string.Empty;

	[JsonPropertyName("notas")]
	public string? Notas { get; set; }

	[JsonPropertyName("metodo_pago_id")]
	public int? MetodoPagoId { get; set; }

	[JsonPropertyName("metodo_pago")]
	public string? MetodoPago { get; set; }

	[JsonPropertyName("Mascotum")]
	public MascotaResumenDTO? Mascotum { get; set; }

	[JsonPropertyName("doctor")]
	public UsuarioResumenDTO? Doctor { get; set; }

	[JsonPropertyName("secretaria")]
	[JsonIgnore(Condition = JsonIgnoreCondition.Never)]
	public UsuarioResumenDTO? Secretaria { get; set; }
}

public class CitasNotificationDTO
{
	[JsonPropertyName("fecha_cita")]
	public string FechaCita { get; set; } = null!;
	[JsonPropertyName("hora_cita")]
	public string HoraCita { get; set; } = null!;
	[JsonPropertyName("estado")]
	public string Estado { get; set; } = string.Empty;

}