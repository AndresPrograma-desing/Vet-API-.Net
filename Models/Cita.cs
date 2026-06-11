using System;
using System.Collections.Generic;

namespace vet_api_Net.Models;

public partial class Cita
{
    public int Id { get; set; }

    public int MascotaId { get; set; }

    public int DoctorId { get; set; }

    public int? SecretariaId { get; set; }

    public DateTime FechaCita { get; set; }

    public TimeOnly HoraCita { get; set; }

    public string Motivo { get; set; } = null!;

    public string? TipoCita { get; set; }

    public string? Estado { get; set; }

    public string? Notas { get; set; }

    public int? MetodoPagoId { get; set; }

    public virtual MetodoPago? MetodoPago { get; set; }

    public virtual ICollection<Consulta> Consulta { get; set; } = new List<Consulta>();

    public virtual Usuario Doctor { get; set; } = null!;

    public virtual Mascota Mascota { get; set; } = null!;

    public virtual Usuario? Secretaria { get; set; }
}
