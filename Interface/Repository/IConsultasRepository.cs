using System.Threading.Tasks;
using System.Collections.Generic;
using vet_api_Net.Models;
using DTOs;

namespace vet_api_Net.Interfaze.Repositories;

public interface IConsultasRepository
{
    Task<Cita?> GetCitaByIdAsync(int id);
    Task<Mascota?> GetMascotaByIdAsync(int id);
    Task<Usuario?> GetDoctorByIdAsync(int id);
    Task<Producto?> GetProductoByIdAsync(int id);
    Task<MetodoPago?> GetMetodoPagoByIdAsync(int id);
    Task<Consulta?> GetConsultaWithProductsAsync(int id);
    void AddConsulta(Consulta medicalRecord);
    void AddConsultaProducto(ConsultasProducto detail);
    void AddFactura(Factura invoice);
    void AddDetalleFactura(DetallesFactura invoiceDetail);
    Task<bool> SaveChangesAsync();
    Task<ConsultaRequestDTO?> GetConsultaDtoByIdAsync(int id);
    Task<IEnumerable<ConsultaRequestDTO>> GetConsultasAsync();
    Task<Consulta?> GetConsultaByIdEntityAsync(int id);
    
}