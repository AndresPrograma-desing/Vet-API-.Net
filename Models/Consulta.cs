using System;
using System.Collections.Generic;

namespace vet_api_Net.Models;

public partial class Consulta
{
    public int Id { get; set; }

    public int CitaId { get; set; }

    public int MascotaId { get; set; }

    public int DoctorId { get; set; }

    public DateTime FechaConsulta { get; set; }

    public decimal? PesoActual { get; set; }

    public decimal? Temperatura { get; set; }

    public string Sintomas { get; set; } = null!;

    public string? Diagnostico { get; set; }

    public string? Tratamiento { get; set; }

    public string? Receta { get; set; }

    public string? Observaciones { get; set; }

    public decimal? ConsultaPrice { get; set; }

    public DateTime Creado { get; set; }

    public virtual Cita Cita { get; set; } = null!;

    public virtual ICollection<ConsultasProducto> ConsultasProductos { get; set; } = new List<ConsultasProducto>();

    public virtual Usuario Doctor { get; set; } = null!;

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual Mascota Mascota { get; set; } = null!;

    public virtual ICollection<Vacuna> Vacunas { get; set; } = new List<Vacuna>();
}
