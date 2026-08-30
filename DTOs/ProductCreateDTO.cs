using System;
using System.ComponentModel.DataAnnotations;
using vet_api_Net.Constants;

namespace DTOs;

public record ProductCreateDTO
{
    [Required(ErrorMessage = ResponseMessagesProduct.ResponseMessagesProductCreate.NameAndCodeRequired)]
    [StringLength(50, ErrorMessage = ResponseMessagesDtos.Producto.CodigoMaxLength)]
    public string Codigo { get; set; } = null!;

    [Required(ErrorMessage = ResponseMessagesProduct.ResponseMessagesProductCreate.NameAndCodeRequired)]
    [StringLength(200, ErrorMessage = ResponseMessagesDtos.Producto.NombreMaxLength)]
    public string Nombre { get; set; } = null!;

    [StringLength(500, ErrorMessage = ResponseMessagesDtos.Producto.DescripcionMaxLength)]
    public string? Descripcion { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = ResponseMessagesDtos.Producto.CategoriaIdInvalid)]
    public int? CategoriaId { get; set; }

    [Required(ErrorMessage = ResponseMessagesDtos.Producto.TipoRequired)]
    [StringLength(50, ErrorMessage = ResponseMessagesDtos.Producto.TipoMaxLength)]
    public string Tipo { get; set; } = null!;

    [Range(0, double.MaxValue, ErrorMessage = ResponseMessagesProduct.ResponseMessagesProductCreate.PriceCannotBeNegative)]
    public decimal Precio { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = ResponseMessagesProduct.ResponseMessagesProductCreate.PriceCannotBeNegative)]
    public decimal PrecioVenta { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = ResponseMessagesDtos.Producto.StockNoNegativo)]
    public int? Stock { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = ResponseMessagesDtos.Producto.StockMinimoNoNegativo)]
    public int? StockMinimo { get; set; }

    [StringLength(50, ErrorMessage = ResponseMessagesDtos.Producto.UnidadMedidaMaxLength)]
    public string? UnidadMedida { get; set; }

    [StringLength(200, ErrorMessage = ResponseMessagesDtos.Producto.ProveedorMaxLength)]
    public string? Proveedor { get; set; }
}