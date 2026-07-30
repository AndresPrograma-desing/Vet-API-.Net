using System;
using System.Text.Json.Serialization;

namespace DTOs;

using System.Text.Json.Serialization;
public record ConsultaRequestDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("cita_id")]
    public int CitaId { get; set; }

    [JsonPropertyName("mascota_id")]
    public int MascotaId { get; set; }

    [JsonPropertyName("doctor_id")]
    public int DoctorId { get; set; }

    [JsonPropertyName("fecha_consulta")]
    public DateTime? FechaConsulta { get; set; }

    [JsonPropertyName("peso_actual")]
    public decimal? PesoActual { get; set; }

    [JsonPropertyName("temperatura")]
    public decimal? Temperatura { get; set; }

    [JsonPropertyName("sintomas")]
    public string Sintomas { get; set; } = null!;

    [JsonPropertyName("diagnostico")]
    public string? Diagnostico { get; set; }

    [JsonPropertyName("tratamiento")]
    public string? Tratamiento { get; set; }

    [JsonPropertyName("receta")]
    public string? Receta { get; set; }
    
    [JsonPropertyName("consulta_price")]
    public decimal? ConsultaPrice { get; set; }

    [JsonPropertyName("observaciones")]
    public string? Observaciones { get; set; }

    [JsonPropertyName("mascota_nombre")]
    public string? MascotaNombre { get; set; }

    [JsonPropertyName("cliente_telefono")]
    public string? ClienteTelefono { get; set; }

    [JsonPropertyName("telefono_cliente")]
    public string? TelefonoCliente { get; set; }

    [JsonPropertyName("correo_cliente")]
    public string? CorreoCliente { get; set; }

    [JsonPropertyName("creado")]
    public DateTime Creado { get; set; }

    // Al declarar esta propiedad de última, aparecerá al final del JSON
    [JsonPropertyName("productos")]
    public List<ConsultaProductoDetalleDTO> Productos { get; set; } = new();
}

public class ConsultaProductoDetalleDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("producto_id")]
    public int ProductoId { get; set; }

    [JsonPropertyName("nombre_producto")]
    public string? NombreProducto { get; set; }

    [JsonPropertyName("cantidad")]
    public int Cantidad { get; set; }

    [JsonPropertyName("precio_unitario")]
    public decimal PrecioUnitario { get; set; }

    [JsonPropertyName("dosis")]
    public string? Dosis { get; set; }

    [JsonPropertyName("via_administracion")]
    public string? ViaAdministracion { get; set; }

    [JsonPropertyName("frecuencia")]
    public string? Frecuencia { get; set; }

    [JsonPropertyName("duracion")]
    public string? Duracion { get; set; }

    [JsonPropertyName("instrucciones")]
    public string? Instrucciones { get; set; }
}