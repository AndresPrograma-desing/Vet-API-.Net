using System;
using System.Collections.Generic;

namespace vet_api_Net.Models;

public partial class MetodoPago
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public DateTime Creado { get; set; }

    public DateTime Actualizado { get; set; }

    public virtual ICollection<Cita> Citas { get; set; } = new List<Cita>();
}
