using System;
using System.Collections.Generic;

namespace vet_api_Net.Models;

public partial class Cliente
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? Direccion { get; set; }

    public string? Identificacion { get; set; }

    public string? Nota { get; set; }

    public DateTime Creado { get; set; }

    public DateTime Actualizado { get; set; }

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual ICollection<Mascota> Mascota { get; set; } = new List<Mascota>();
}
