using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Models;
using vet_api_Net.Constants;

namespace vet_api_Net.Services
{
	public class ReportSystemService : IReportSystemService
	{
		private readonly AppDbContext _context;

		public ReportSystemService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Reporte>> GetAllAsync()
		{
			return await _context.Reportes.AsNoTracking().ToListAsync();
		}

		public async Task<Reporte> GetByIdAsync(int id)
		{
			return await _context.Reportes.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
		}

		public async Task<Reporte> CreateAsync(Reporte reporte)
		{
			_context.Reportes.Add(reporte);
			await _context.SaveChangesAsync();
			return reporte;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var reporte = await _context.Reportes.FindAsync(id);
			if (reporte == null)
			{
				return false;
			}

			_context.Reportes.Remove(reporte);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<Reporte> GenerateFullSystemReportAsync(string generadoPor)
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

			var data = new
			{
				FechaGeneracion = DateTime.UtcNow,
				Clientes = clientes,
				Mascotas = mascotas,
				Productos = productos,
				Facturas = facturas,
				Usuarios = usuarios
			};

			string jsonData = System.Text.Json.JsonSerializer.Serialize(data);

			var reporte = new Reporte
			{
				Titulo = ResponseMessagesReport.ReportTittle,
				FechaCreacion = DateTime.UtcNow,
				Categoria = ResponseMessagesReport.ResportCategory,
				Filtro = ResponseMessagesReport.Filtre,
				Datos = jsonData,
				GeneradoPor = string.IsNullOrWhiteSpace(generadoPor) ? "sistema" : generadoPor
			};

			_context.Reportes.Add(reporte);
			await _context.SaveChangesAsync();
			return reporte;
		}
		
		public async Task<object?> IsEnabledAsync()
{
    return await _context.ReporConfigs
        .AsNoTracking()
        .Select(c => new 
        { 
            c.Days, 
            c.IsEnabled,
			c.GenerateEnabled
        })
        .FirstOrDefaultAsync();
}
	}
}
