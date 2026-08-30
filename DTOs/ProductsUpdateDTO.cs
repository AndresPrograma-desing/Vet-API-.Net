using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using vet_api_Net.Constants;

namespace DTOs;

public record ProductsUpdateDTO
{
    [JsonPropertyName("codigo")]
    [StringLength(50, ErrorMessage = ResponseMessagesDtos.Producto.CodigoMaxLength)]
    public string? Codigo { get; set; }

    [JsonPropertyName("nombre")]
    [StringLength(200, ErrorMessage = ResponseMessagesDtos.Producto.NombreMaxLength)]
    public string? Nombre { get; set; }

    [JsonPropertyName("descripcion")]
    [StringLength(500, ErrorMessage = ResponseMessagesDtos.Producto.DescripcionMaxLength)]
    public string? Descripcion { get; set; }

    [JsonPropertyName("categoria_id")]
    [Range(1, int.MaxValue, ErrorMessage = ResponseMessagesDtos.Producto.CategoriaIdInvalid)]
    public int? CategoriaId { get; set; }

    [JsonPropertyName("tipo")]
    [StringLength(50, ErrorMessage = ResponseMessagesDtos.Producto.TipoMaxLength)]
    public string? Tipo { get; set; }

    [JsonPropertyName("precio")]
    [Range(0, double.MaxValue, ErrorMessage = ResponseMessagesProduct.ResponseMessagesProductCreate.PriceCannotBeNegative)]
    public decimal? Precio { get; set; }

    [JsonPropertyName("precio_venta")]
    [Range(0, double.MaxValue, ErrorMessage = ResponseMessagesProduct.ResponseMessagesProductCreate.PriceCannotBeNegative)]
    public decimal? PrecioVenta { get; set; }

    [JsonPropertyName("stock")]
    [Range(0, int.MaxValue, ErrorMessage = ResponseMessagesDtos.Producto.StockNoNegativo)]
    public int? Stock { get; set; }

    [JsonPropertyName("stock_minimo")]
    [Range(0, int.MaxValue, ErrorMessage = ResponseMessagesDtos.Producto.StockMinimoNoNegativo)]
    public int? StockMinimo { get; set; }

    [JsonPropertyName("unidad_medida")]
    [StringLength(50, ErrorMessage = ResponseMessagesDtos.Producto.UnidadMedidaMaxLength)]
    public string? UnidadMedida { get; set; }

    [JsonPropertyName("proveedor")]
    [StringLength(200, ErrorMessage = ResponseMessagesDtos.Producto.ProveedorMaxLength)]
    public string? Proveedor { get; set; }
}