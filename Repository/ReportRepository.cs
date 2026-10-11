using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Constants;
using vet_api_Net.Data;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Models;

//Describe: Acceso a datos de los reportes del sistema y construcción de la instantánea completa de datos.
namespace vet_api_Net.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly AppDbContext _context;

    public ReportRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Reporte>> GetAllAsync()
        => await _context.Reportes.AsNoTracking().ToListAsync();

    public async Task<Reporte?> GetByIdAsync(int id)
        => await _context.Reportes.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);

    public async Task<Reporte?> GetByIdTrackedAsync(int id)
        => await _context.Reportes.FindAsync(id);

    public async Task<List<Reporte>> GetExpiredAsync(DateTime deadline, CancellationToken cancellationToken = default)
        => await _context.Reportes
            .Where(r => r.FechaCreacion <= deadline)
            .ToListAsync(cancellationToken);

    public async Task<Reporte?> GetLastByGeneratorAsync(string generatedBy, CancellationToken cancellationToken = default)
        => await _context.Reportes
            .Where(r => r.GeneradoPor == generatedBy)
            .OrderByDescending(r => r.FechaCreacion)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task AddAsync(Reporte reporte)
        => await _context.Reportes.AddAsync(reporte);

    public void Remove(Reporte reporte)
        => _context.Reportes.Remove(reporte);

    public void RemoveRange(IEnumerable<Reporte> reportes)
        => _context.Reportes.RemoveRange(reportes);

    public async Task<(List<object> Clientes, List<object> Mascotas, List<object> Productos, List<object> Facturas, List<object> Usuarios, List<object> Appointments, List<object> Consultations, List<object> InventoryMovements, object Summary)> GetSystemSnapshotAsync()
    {
        var clientes = await _context.Clientes
            .AsNoTracking()
            .Select(c => (object)new
            {
                c.Id,
                c.Nombre,
                c.Apellido,
                c.Email,
                c.Telefono,
                c.Direccion,
                c.Identificacion,
                c.Creado,
                c.Actualizado
            })
            .ToListAsync();

        var mascotas = await _context.Mascotas
            .AsNoTracking()
            .Select(m => (object)new
            {
                m.Id,
                m.ClienteId,
                m.Nombre,
                Especie = m.Especie.Nombre,
                m.Raza,
                m.Sexo,
                m.FechaNacimiento,
                m.Peso,
                m.Creado,
                m.Actualizado
            })
            .ToListAsync();

        var productos = await _context.Productos
            .AsNoTracking()
            .Select(p => (object)new
            {
                p.Id,
                p.Codigo,
                p.Nombre,
                p.Tipo,
                p.Precio,
                p.PrecioVenta,
                p.Stock,
                p.StockMinimo,
                p.Proveedor,
                p.Creado,
                p.Actualizado
            })
            .ToListAsync();

        var facturas = await _context.Facturas
            .AsNoTracking()
            .Select(f => (object)new
            {
                f.Id,
                f.NumeroFactura,
                f.ClienteId,
                f.MascotaId,
                f.ConsultaId,
                f.SecretariaId,
                f.FechaEmision,
                f.Subtotal,
                f.Descuento,
                f.Total,
                f.MetodoPago,
                f.EstadoPago,
                f.Creado,
                f.Actualizado
            })
            .ToListAsync();

        var usuarios = await _context.Usuarios
            .AsNoTracking()
            .Select(u => (object)new
            {
                u.Id,
                u.Nombre,
                u.Apellido,
                u.Email,
                u.Rol,
                u.Activo,
                u.UltimoAcceso,
                u.Creado,
                u.Actualizado
            })
            .ToListAsync();

        var appointments = await _context.Citas
            .AsNoTracking()
            .Select(c => (object)new
            {
                c.Id,
                c.MascotaId,
                c.DoctorId,
                c.SecretariaId,
                c.FechaCita,
                c.HoraCita,
                c.Motivo,
                c.TipoCita,
                c.Estado
            })
            .ToListAsync();

        var consultations = await _context.Consultas
            .AsNoTracking()
            .Select(c => (object)new
            {
                c.Id,
                c.CitaId,
                c.MascotaId,
                c.DoctorId,
                c.FechaConsulta,
                c.Diagnostico,
                c.Tratamiento,
                c.ConsultaPrice,
                c.Creado
            })
            .ToListAsync();

        var inventoryMovements = await _context.MovimientosInventarios
            .AsNoTracking()
            .Select(m => (object)new
            {
                m.Id,
                m.ProductoId,
                Producto = m.Producto.Nombre,
                m.TipoMovimiento,
                m.Cantidad,
                m.StockAnterior,
                m.StockNuevo,
                m.Motivo,
                m.FechaMovimiento
            })
            .ToListAsync();

        var totalRevenue = await _context.Facturas.SumAsync(f => f.Total);
        var pendingInvoicesCount = await _context.Facturas.CountAsync(f => f.EstadoPago == Status.Pending);
        var totalAppointments = await _context.Citas.CountAsync();
        var pendingAppointments = await _context.Citas.CountAsync(c => c.Estado == Status.Pending);
        var completedAppointments = await _context.Citas.CountAsync(c => c.Estado == Status.Completed);
        var cancelledAppointments = await _context.Citas.CountAsync(c => c.Estado == Status.Cancelled);
        var notAssistedAppointments = await _context.Citas.CountAsync(c => c.Estado == Status.NotAssisted);
        var lowStockProductsCount = await _context.Productos.CountAsync(p => p.Stock <= p.StockMinimo);

        var summary = new
        {
            GeneratedAt = DateTime.UtcNow,
            TotalClients = clientes.Count,
            TotalPets = mascotas.Count,
            TotalProducts = productos.Count,
            TotalUsers = usuarios.Count,
            TotalInvoices = facturas.Count,
            TotalRevenue = totalRevenue,
            PendingInvoicesCount = pendingInvoicesCount,
            TotalAppointments = totalAppointments,
            PendingAppointments = pendingAppointments,
            CompletedAppointments = completedAppointments,
            CancelledAppointments = cancelledAppointments,
            NotAssistedAppointments = notAssistedAppointments,
            TotalConsultations = consultations.Count,
            TotalInventoryMovements = inventoryMovements.Count,
            LowStockProductsCount = lowStockProductsCount
        };

        return (clientes, mascotas, productos, facturas, usuarios, appointments, consultations, inventoryMovements, summary);
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken) > 0;
}
