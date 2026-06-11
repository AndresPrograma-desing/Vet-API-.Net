using System;
using System.Collections.Generic;

namespace vet_api_Net.Models;

public partial class ConsultasProducto
{
    public int Id { get; set; }

    public int ConsultaId { get; set; }

    public int ProductoId { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public string? Dosis { get; set; }

    public string? ViaAdministracion { get; set; }

    public string? Frecuencia { get; set; }

    public string? Duracion { get; set; }

    public string? Instrucciones { get; set; }

    public int? AplicadoPor { get; set; }

    public DateTime Creado { get; set; }

    public virtual Usuario? AplicadoPorNavigation { get; set; }

    public virtual Consulta Consulta { get; set; } = null!;

    public virtual ICollection<DetallesFactura> DetallesFacturas { get; set; } = new List<DetallesFactura>();

    public virtual Producto Producto { get; set; } = null!;
}
