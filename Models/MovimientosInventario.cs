using System;
using System.Collections.Generic;

namespace vet_api_Net.Models;

public partial class MovimientosInventario
{
    public int Id { get; set; }

    public int ProductoId { get; set; }

    public string TipoMovimiento { get; set; } = null!;

    public int Cantidad { get; set; }

    public int StockAnterior { get; set; }

    public int StockNuevo { get; set; }

    public string Motivo { get; set; } = null!;

    public int? ReferenciaId { get; set; }

    public int UsuarioId { get; set; }

    public DateTime FechaMovimiento { get; set; }

    public virtual Producto Producto { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
