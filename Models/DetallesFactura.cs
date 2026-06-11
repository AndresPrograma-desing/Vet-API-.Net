using System;
using System.Collections.Generic;

namespace vet_api_Net.Models;

public partial class DetallesFactura
{
    public int Id { get; set; }

    public int FacturaId { get; set; }

    public int ProductoId { get; set; }

    public int? ProductosConsultasId { get; set; }

    public string? Descripcion { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Total { get; set; }

    public DateTime Created { get; set; }

    public virtual Factura Factura { get; set; } = null!;

    public virtual Producto Producto { get; set; } = null!;

    public virtual ConsultasProducto? ProductosConsultas { get; set; }
}
