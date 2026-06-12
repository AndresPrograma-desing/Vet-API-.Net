using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaces.Repositories;

public interface IFacturasRepository
{
    Task<Cita?> GetCitaWithClienteAsync(int citaId);
    Task<Consulta?> GetConsultaForInvoiceAsync(int citaId, int mascotaId);
    Task<List<DetallesFactura>> GetDetallesByConsultaIdAsync(int consultaId);
    Task<List<DetallesFactura>> GetDetallesByFacturaIdAsync(int facturaId);
    Task<Factura?> GetFacturaByConsultaIdAsync(int consultaId);
    Task<Factura?> GetFacturaByIdAsync(int facturaId);
    Task<List<int>> GetMascotaIdsByClienteIdAsync(int clienteId);
    Task<List<Consulta>> GetConsultasWithRelationsAsync(List<int> mascotaIds);
    Task<List<Factura>> GetAllFacturasWithRelationsAsync();
    Task<FacturaConfig?> GetFirstFacturaConfigAsync();
    Task<Cita?> GetCitaByIdAsync(int citaId);
    Task<MetodoPago?> GetMetodoPagoByIdAsync(int metodoPagoId);
    Task<int> GetFirstUsuarioIdAsync();
    void AddFactura(Factura factura);
    void UpdateFactura(Factura factura);
    Task<bool> SaveChangesAsync();
}