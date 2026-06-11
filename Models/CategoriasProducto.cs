using System;
using System.Collections.Generic;

namespace vet_api_Net.Models;

public partial class CategoriasProducto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool? Activo { get; set; }

    public DateTime Creado { get; set; }

    public DateTime Actualizado { get; set; }

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
