using System;
using System.Collections.Generic;

namespace vet_api_Net.Models;

public partial class Factura
{
    public int Id { get; set; }

    public string NumeroFactura { get; set; } = null!;

    public int ClienteId { get; set; }

    public int? MascotaId { get; set; }

    public int? ConsultaId { get; set; }

    public int SecretariaId { get; set; }

    public DateTime FechaEmision { get; set; }

    public decimal Subtotal { get; set; }

    public decimal? Descuento { get; set; }

    public decimal Total { get; set; }

    public string MetodoPago { get; set; } = null!;

    public string? EstadoPago { get; set; }

    public string? Notas { get; set; }

    public string? UrlDocx { get; set; }

    public DateTime Creado { get; set; }

    public DateTime Actualizado { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual Consulta? Consulta { get; set; }

    public virtual ICollection<DetallesFactura> DetallesFacturas { get; set; } = new List<DetallesFactura>();

    public virtual Mascota? Mascota { get; set; }

    public virtual Usuario Secretaria { get; set; } = null!;
}
