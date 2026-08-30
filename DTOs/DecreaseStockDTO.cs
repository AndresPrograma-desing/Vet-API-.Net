using System.ComponentModel.DataAnnotations;
using vet_api_Net.Constants;

namespace DTOs;

public record DecreaseStockDTO
{
    [Range(1, int.MaxValue, ErrorMessage = ResponseMessagesProduct.ProductCantPositive)]
    public int Quantity { get; set; }
}