using System;
using System.Collections.Generic;

namespace vet_api_Net.Models;

public partial class Vacuna
{
    public int Id { get; set; }

    public int MascotaId { get; set; }

    public int ProductoId { get; set; }

    public int? ConsultaId { get; set; }

    public DateOnly FechaVacunacion { get; set; }

    public DateOnly? ProximaDosis { get; set; }

    public string? Lote { get; set; }

    public int? DoctorId { get; set; }

    public string? Nota { get; set; }

    public DateTime Creado { get; set; }

    public virtual Consulta? Consulta { get; set; }

    public virtual Usuario? Doctor { get; set; }

    public virtual Mascota Mascota { get; set; } = null!;

    public virtual Producto Producto { get; set; } = null!;
}
