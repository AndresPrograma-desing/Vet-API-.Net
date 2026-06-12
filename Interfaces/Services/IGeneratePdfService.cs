using DTOs;

namespace vet_api_Net.Interfaces.Services
{
    public interface IGeneratePdfService
    {
        string GenerateInvoicePdf(FacturationDTO invoice, string webRootPath, string currencySymbol);
    }
}
