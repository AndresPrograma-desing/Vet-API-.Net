using System;
using System.Text.Json.Serialization;

namespace DTOs;
 
public record DetailsCitaDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = null!;

    [JsonPropertyName("apellido")]
    public string Apellido { get; set; } = null!;

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("telefono")]
    public string? Telefono { get; set; }

    [JsonPropertyName("identificacion")]
    public string? Identificacion { get; set; }
}
 
public record MascotaResumenDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("cliente_id")]
    public int ClienteId { get; set; }

    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = null!;

    [JsonPropertyName("especie")]
    public string Especie { get; set; } = null!;

    [JsonPropertyName("raza")]
    public string? Raza { get; set; }

    [JsonPropertyName("sexo")]
    public string? Sexo { get; set; }

    [JsonPropertyName("fecha_nacimiento")]
    public string? FechaNacimiento { get; set; }

    [JsonPropertyName("peso")]
    public string? Peso { get; set; }

    [JsonPropertyName("identeficacion_mascota")]
    public string? IdenteficacionMascota { get; set; }

    [JsonPropertyName("color")]
    public string? Color { get; set; }

    [JsonPropertyName("alergias")]
    public string? Alergias { get; set; }

    [JsonPropertyName("condiciones_medicas")]
    public string? CondicionesMedicas { get; set; }

    [JsonPropertyName("esterilizado")]
    public bool? Esterilizado { get; set; }

    [JsonPropertyName("Cliente")]
    public DetailsCitaDTO? Cliente { get; set; }
    
    [JsonPropertyName("creado")]
    public DateTime? Creado { get; set; }

    [JsonPropertyName("actualizado")]
    public DateTime? Actualizado { get; set; }

    [JsonPropertyName("consultas")]
    public List<ConsultaResumenDTO> Consultas { get; set; } = new List<ConsultaResumenDTO>();

    [JsonPropertyName("citas")]
    public List<CitaResumenDTO> Citas { get; set; } = new List<CitaResumenDTO>();
}
 
public record UsuarioResumenDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = null!;

    [JsonPropertyName("apellido")]
    public string Apellido { get; set; } = null!;

    [JsonPropertyName("rol")]
    public string Rol { get; set; } = null!;
}

public record CitaDetalleDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("fecha_cita")]
    public DateTime FechaCita { get; set; }

    [JsonPropertyName("hora_cita")]
    public TimeSpan HoraCita { get; set; }

    [JsonPropertyName("motivo")]
    public string Motivo { get; set; } = null!;

    [JsonPropertyName("tipo_cita")]
    public string TipoCita { get; set; } = null!;

    [JsonPropertyName("estado")]
    public string Estado { get; set; } = null!;

    [JsonPropertyName("notas")]
    public string? Notas { get; set; }

    // Relaciones usando los DTOs anteriores
    [JsonPropertyName("mascota")]
    public MascotaResumenDTO Mascota { get; set; } = null!;

    [JsonPropertyName("doctor")]
    public UsuarioResumenDTO Doctor { get; set; } = null!;

    [JsonPropertyName("secretaria")]
    public UsuarioResumenDTO? Secretaria { get; set; } // Opcional (null si no hay)

    [JsonPropertyName("metodo_pago_id")]
    public int? MetodoPagoId { get; set; }

    [JsonPropertyName("metodo_pago")]
    public string? MetodoPago { get; set; }
}

public record ConsultaResumenDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("fecha")]
    public DateTime Fecha { get; set; }
}

public record CitaResumenDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("fecha")]
    public DateTime Fecha { get; set; }
}