using System;
using System.Collections.Generic;

namespace vet_api_Net.Models;

public partial class HistorialPrecio
{
    public int Id { get; set; }

    public int ProductoId { get; set; }

    public decimal PrecioAnterior { get; set; }

    public decimal PrecioNuevo { get; set; }

    public int UsuarioId { get; set; }

    public string? Motivo { get; set; }

    public DateTime FechaCambio { get; set; }

    public virtual Producto Producto { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
