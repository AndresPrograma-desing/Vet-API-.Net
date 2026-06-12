using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaces.Repositories;

public interface IInvoiceExternalRepository
{
    Task<Cita?> GetCitaWithDetailsAsync(int citaId);
    Task<Consulta?> GetConsultaByCitaOrPetAsync(int citaId, int mascotaId);
    Task<MoneyType?> GetActiveMoneyTypeAsync(int targetId);
    Task<Factura?> GetFacturaByConsultaIdAsync(int consultaId);
    Task<List<DetallesFactura>> GetDetallesFacturaAsync(int consultaId);
}