using System.Threading.Tasks;
using DTOs;

namespace vet_api_Net.Interfaces.Services;

public interface IInvoiceExternalService
{
    Task<InvoiceDispatchResponseDTO> VerifyAndDispatchInvoiceByCitaAsync(int citaId);
}