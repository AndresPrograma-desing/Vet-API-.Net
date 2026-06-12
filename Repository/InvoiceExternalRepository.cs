using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Models;
using vet_api_Net.Interfaces.Repositories;

namespace vet_api_Net.Repositories;

public class InvoiceExternalRepository : IInvoiceExternalRepository
{
    private readonly AppDbContext _context;

    public InvoiceExternalRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Cita?> GetCitaWithDetailsAsync(int citaId)
    {
        return await _context.Citas
            .Include(c => c.Mascota).ThenInclude(m => m.Cliente)
            .FirstOrDefaultAsync(c => c.Id == citaId);
    }

    public async Task<Consulta?> GetConsultaByCitaOrPetAsync(int citaId, int mascotaId)
    {
        var consulta = await _context.Consultas
            .Where(c => c.CitaId == citaId)
            .Include(c => c.Mascota).ThenInclude(m => m.Cliente)
            .Include(c => c.ConsultasProductos).ThenInclude(cp => cp.Producto)
            .OrderByDescending(c => c.FechaConsulta)
            .FirstOrDefaultAsync();

        if (consulta == null)
        {
            consulta = await _context.Consultas
                .Where(c => c.MascotaId == mascotaId)
                .Include(c => c.Mascota).ThenInclude(m => m.Cliente)
                .Include(c => c.ConsultasProductos).ThenInclude(cp => cp.Producto)
                .OrderByDescending(c => c.FechaConsulta)
                .FirstOrDefaultAsync();
        }

        return consulta;
    }

    public async Task<MoneyType?> GetActiveMoneyTypeAsync(int targetId)
    {
        return await _context.MoneyTypes.FirstOrDefaultAsync(m => m.Id == targetId);
    }

    public async Task<Factura?> GetFacturaByConsultaIdAsync(int consultaId)
    {
        return await _context.Facturas.FirstOrDefaultAsync(f => f.ConsultaId == consultaId);
    }

    public async Task<List<DetallesFactura>> GetDetallesFacturaAsync(int consultaId)
    {
        return await _context.DetallesFacturas
            .Where(d => d.ProductosConsultasId != null)
            .Include(d => d.Producto)
            .Include(d => d.ProductosConsultas)
            .Where(d => d.ProductosConsultas != null && d.ProductosConsultas!.ConsultaId == consultaId)
            .ToListAsync();
    }
}