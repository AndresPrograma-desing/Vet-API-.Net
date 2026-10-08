using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

    public async Task<(List<object> Clientes, List<object> Mascotas, List<object> Productos, List<object> Facturas, List<object> Usuarios)> GetSystemSnapshotAsync()
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

        return (clientes, mascotas, productos, facturas, usuarios);
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken) > 0;
}
