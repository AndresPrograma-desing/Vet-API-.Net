namespace DTOs;

public record InvoiceDispatchResponseDTO
{
    public int InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = null!;
    public bool IsDispatched { get; set; }
    public string ClientName { get; set; } = null!;
    public string DestinationPhone { get; set; } = null!;
    public decimal TotalCalculated { get; set; }
    public string CurrencyUsed { get; set; } = null!;
    public string ClientId { get; set; } = null!;
}