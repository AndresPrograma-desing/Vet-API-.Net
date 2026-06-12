using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Models;
using vet_api_Net.Interfaces.Repositories;

namespace vet_api_Net.Repositories;

public class FacturasRepository : IFacturasRepository
{
    private readonly AppDbContext _db;

    public FacturasRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Cita?> GetCitaWithClienteAsync(int citaId)
    {
        return await _db.Citas
            .Include(c => c.Mascota).ThenInclude(m => m.Cliente)
            .FirstOrDefaultAsync(c => c.Id == citaId);
    }

    public async Task<Consulta?> GetConsultaForInvoiceAsync(int citaId, int mascotaId)
    {
        return await _db.Consultas
            .Where(c => c.CitaId == citaId)
            .Include(c => c.Mascota).ThenInclude(m => m.Cliente)
            .Include(c => c.Doctor)
            .Include(c => c.ConsultasProductos).ThenInclude(cp => cp.Producto)
            .OrderByDescending(c => c.FechaConsulta)
            .FirstOrDefaultAsync() 
            ?? await _db.Consultas
            .Where(c => c.MascotaId == mascotaId)
            .Include(c => c.Mascota).ThenInclude(m => m.Cliente)
            .Include(c => c.Doctor)
            .Include(c => c.ConsultasProductos).ThenInclude(cp => cp.Producto)
            .OrderByDescending(c => c.FechaConsulta)
            .FirstOrDefaultAsync();
    }

    public async Task<List<DetallesFactura>> GetDetallesByConsultaIdAsync(int consultaId)
    {
        return await _db.DetallesFacturas
            .Where(d => d.ProductosConsultasId != null)
            .Include(d => d.Producto)
            .Include(d => d.ProductosConsultas)
            .Where(d => d.ProductosConsultas != null && d.ProductosConsultas.ConsultaId == consultaId)
            .ToListAsync();
    }

    public async Task<List<DetallesFactura>> GetDetallesByFacturaIdAsync(int facturaId)
    {
        return await _db.DetallesFacturas
            .Where(d => d.FacturaId == facturaId)
            .Include(d => d.Producto)
            .ToListAsync();
    }

    public async Task<Factura?> GetFacturaByConsultaIdAsync(int consultaId)
    {
        return await _db.Facturas.AsNoTracking().FirstOrDefaultAsync(f => f.ConsultaId == consultaId);
    }

    public async Task<Factura?> GetFacturaByIdAsync(int facturaId)
    {
        return await _db.Facturas
            .Include(f => f.Consulta).ThenInclude(c => c!.Mascota).ThenInclude(m => m.Cliente)
            .Include(f => f.Consulta).ThenInclude(c => c!.Doctor)
            .Include(f => f.Consulta).ThenInclude(c => c!.ConsultasProductos).ThenInclude(cp => cp.Producto)
            .FirstOrDefaultAsync(f => f.Id == facturaId);
    }

    public async Task<List<int>> GetMascotaIdsByClienteIdAsync(int clienteId)
    {
        return await _db.Mascotas.Where(m => m.ClienteId == clienteId).Select(m => m.Id).ToListAsync();
    }

    public async Task<List<Consulta>> GetConsultasWithRelationsAsync(List<int> mascotaIds)
    {
        return await _db.Consultas
            .Where(c => mascotaIds.Contains(c.MascotaId))
            .Include(c => c.Mascota).ThenInclude(m => m.Cliente)
            .Include(c => c.Doctor)
            .Include(c => c.ConsultasProductos).ThenInclude(cp => cp.Producto)
            .OrderBy(c => c.FechaConsulta)
            .ToListAsync();
    }

    public async Task<List<Factura>> GetAllFacturasWithRelationsAsync()
    {
        return await _db.Facturas
            .Include(f => f.Consulta).ThenInclude(c => c!.Mascota).ThenInclude(m => m.Cliente)
            .Include(f => f.Consulta).ThenInclude(c => c!.Doctor)
            .Include(f => f.Consulta).ThenInclude(c => c!.ConsultasProductos).ThenInclude(cp => cp.Producto)
            .Where(f => f.Consulta != null)
            .OrderByDescending(f => f.FechaEmision)
            .ToListAsync();
    }

    public async Task<FacturaConfig?> GetFirstFacturaConfigAsync()
    {
        return await _db.FacturaConfigs.AsNoTracking().FirstOrDefaultAsync();
    }

    public async Task<Cita?> GetCitaByIdAsync(int citaId)
    {
        return await _db.Citas.FindAsync(citaId);
    }

    public async Task<MetodoPago?> GetMetodoPagoByIdAsync(int metodoPagoId)
    {
        return await _db.MetodoPagos.FindAsync(metodoPagoId);
    }

    public async Task<int> GetFirstUsuarioIdAsync()
    {
        return await _db.Usuarios.Select(u => u.Id).FirstOrDefaultAsync();
    }

    public void AddFactura(Factura factura)
    {
        _db.Facturas.Add(factura);
    }

    public void UpdateFactura(Factura factura)
    {
        _db.Facturas.Update(factura);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _db.SaveChangesAsync() > 0;
    }
}