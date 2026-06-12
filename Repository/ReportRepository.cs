using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Models;
using vet_api_Net.Interfaces.Repositories;

namespace vet_api_Net.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;

        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reporte>> GetAllAsync()
        {
            return await _context.Reportes.AsNoTracking().ToListAsync();
        }

        public async Task<Reporte?> GetByIdAsync(int id)
        {
            return await _context.Reportes.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddAsync(Reporte reporte)
        {
            await _context.Reportes.AddAsync(reporte);
        }

        public void Delete(Reporte reporte)
        {
            _context.Reportes.Remove(reporte);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<object?> GetSystemReportDataAsync()
        {
            var clientes = await _context.Clientes
                .AsNoTracking()
                .Select(c => new
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
                .Select(m => new
                {
                    m.Id,
                    m.ClienteId,
                    m.Nombre,
                    m.Especie,
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
                .Select(p => new
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
                .Select(f => new
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
                .Select(u => new
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

            return new
            {
                FechaGeneracion = DateTime.UtcNow,
                Clientes = clientes,
                Mascotas = mascotas,
                Productos = productos,
                Facturas = facturas,
                Usuarios = usuarios
            };
        }

        public async Task<ReporConfig?> GetConfigAsync()
        {
            return await _context.ReporConfigs.FirstOrDefaultAsync();
        }

        public void AddConfig(ReporConfig config)
        {
            _context.ReporConfigs.Add(config);
        }

        public void UpdateConfig(ReporConfig config)
        {
            _context.ReporConfigs.Update(config);
        }
    }
}
