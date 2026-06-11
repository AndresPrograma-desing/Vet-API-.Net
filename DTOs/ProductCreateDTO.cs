using System;

namespace DTOs;

public record ProductCreateDTO
{
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
}