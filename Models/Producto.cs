using System;
using System.Collections.Generic;

namespace vet_api_Net.Models;

public partial class Producto
{
    public int Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public int? CategoriaId { get; set; }

    public string Tipo { get; set; } = null!;

    public decimal Precio { get; set; }

    public decimal PrecioVenta { get; set; }

    public int? Stock { get; set; }

    public int? StockMinimo { get; set; }

    public string? UnidadMedida { get; set; }

    public string? Proveedor { get; set; }

    public bool? RequiereReceta { get; set; }

    public DateTime Creado { get; set; }

    public DateTime Actualizado { get; set; }

    public virtual CategoriasProducto? Categoria { get; set; }

    public virtual ICollection<ConsultasProducto> ConsultasProductos { get; set; } = new List<ConsultasProducto>();

    public virtual ICollection<DetallesFactura> DetallesFacturas { get; set; } = new List<DetallesFactura>();

    public virtual ICollection<HistorialPrecio> HistorialPrecios { get; set; } = new List<HistorialPrecio>();

    public virtual ICollection<MovimientosInventario> MovimientosInventarios { get; set; } = new List<MovimientosInventario>();

    public virtual ICollection<Vacuna> Vacunas { get; set; } = new List<Vacuna>();
}
