using System;
using System.Collections.Generic;

namespace vet_api_Net.Models;

public partial class Servicio
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal Precio { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime Creado { get; set; } = DateTime.Now;

    public DateTime Actualizado { get; set; } = DateTime.Now;

    public virtual ICollection<DetallesFactura> DetallesFacturas { get; set; } = new List<DetallesFactura>();
}
