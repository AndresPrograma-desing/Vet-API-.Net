using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DTOs;

public record ClientAccountDTO
{
    [JsonPropertyName("client_id")]
    public int ClientId { get; init; }

    [JsonPropertyName("total_paid")]
    public decimal TotalPaid { get; init; }

    [JsonPropertyName("pending_balance")]
    public decimal PendingBalance { get; init; }

    [JsonPropertyName("paid_invoices")]
    public List<FacturationDTO> PaidInvoices { get; init; } = new();

    [JsonPropertyName("pending_invoices")]
    public List<FacturationDTO> PendingInvoices { get; init; } = new();
}
