using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;

namespace vet_api_Net.Interfaze.Services;

public interface IInvoiceService
{
    Task<FacturationDTO?> GenerateInvoiceForCitaAsync(int citaId);
    Task<List<FacturationDTO>> GenerateInvoicesForClientAsync(int clienteId, string targetCurrency = "USD");
    Task<List<FacturationDTO>> AllFacturasAsync();
    Task<FacturationDTO?> ChangeStatusFacturaAsync(int facturaId, string estadoPago, string? metodoPago = null);
    Task<FacturaSettingDTO?> GetFacturaSettingsAsync();
    Task SaveOrUpdateFacturaPersistenceAsync(int citaId, int consultaId, string numeroFactura, string urlDocx, FacturationDTO dto);
}