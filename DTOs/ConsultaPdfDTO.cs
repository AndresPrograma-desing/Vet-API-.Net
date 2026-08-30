using System;
using System.Collections.Generic;

namespace DTOs;

public record ConsultaPdfDTO
{
    public int Id { get; set; }
    public int CitaId { get; set; }
    public int MascotaId { get; set; }
    public int DoctorId { get; set; }
    public DateTime FechaConsulta { get; set; }
    public string Sintomas { get; set; } = string.Empty;
    public string? Diagnostico { get; set; }
    public string? Tratamiento { get; set; }
    public string? Receta { get; set; }
    public string? Observaciones { get; set; }
    public decimal ConsultaPrice { get; set; }
    public int? ClienteId { get; set; }
    public string? ClienteNombre { get; set; }
    public string? ClienteTelefono { get; set; }
    public string? TelefonoCliente { get; set; }
    public string? CorreoCliente { get; set; }
    public string? MascotaNombre { get; set; }
    public List<ConsultaProductoDetalleDTO> Productos { get; set; } = new();
    public List<ConsultaVaccineDetalleDTO> Vacunas { get; set; } = new();
}
