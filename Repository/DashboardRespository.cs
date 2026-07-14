using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Models;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Constants;
using DTOs;

namespace vet_api_Net.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly AppDbContext _context;

    public DashboardRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> GetTotalGananciasAsync(DateTime? startDate, DateTime? endDate)
    {
        var query = _context.Facturas.AsQueryable();
        if (startDate.HasValue) query = query.Where(f => f.FechaEmision >= startDate.Value);
        if (endDate.HasValue) query = query.Where(f => f.FechaEmision <= endDate.Value);

        return await query
            .Where(f => f.EstadoPago != null && 
                        f.EstadoPago.ToLower() != Status.Pending.ToLower() && 
                        f.EstadoPago.ToLower() != Status.Cancelled.ToLower() && 
                        f.EstadoPago.ToLower() != ResponseMessagesDashboard.Cancelado)
            .SumAsync(f => (decimal?)f.Total) ?? 0m;
    }

    public async Task<decimal> GetTotalPerdidasAsync(DateTime? startDate, DateTime? endDate)
    {
        var query = _context.Facturas.AsQueryable();
        if (startDate.HasValue) query = query.Where(f => f.FechaEmision >= startDate.Value);
        if (endDate.HasValue) query = query.Where(f => f.FechaEmision <= endDate.Value);

        return await query
            .Where(f => f.EstadoPago != null && 
                        (f.EstadoPago.ToLower() == Status.Cancelled.ToLower() || 
                         f.EstadoPago.ToLower() == ResponseMessagesDashboard.Cancelado))
            .SumAsync(f => (decimal?)f.Total) ?? 0m;
    }

    public async Task<int> GetTotalFacturasCountAsync(DateTime? startDate, DateTime? endDate)
    {
        var query = _context.Facturas.AsQueryable();
        if (startDate.HasValue) query = query.Where(f => f.FechaEmision >= startDate.Value);
        if (endDate.HasValue) query = query.Where(f => f.FechaEmision <= endDate.Value);

        return await query.CountAsync();
    }

    public async Task<int> GetTotalCitasAsync(DateTime? startDate, DateTime? endDate)
    {
        var query = _context.Citas.AsQueryable();
        if (startDate.HasValue) query = query.Where(c => c.FechaCita >= startDate.Value);
        if (endDate.HasValue) query = query.Where(c => c.FechaCita <= endDate.Value);
        
        return await query.CountAsync();
    }

    public async Task<int> GetTotalMascotasAsync(DateTime? startDate, DateTime? endDate)
    {
        var query = _context.Mascotas.AsQueryable();
        if (startDate.HasValue) query = query.Where(m => m.Creado >= startDate.Value);
        if (endDate.HasValue) query = query.Where(m => m.Creado <= endDate.Value);
        
        return await query.CountAsync();
    }

    public async Task<int> GetTotalClientesAsync(DateTime? startDate, DateTime? endDate)
    {
        var query = _context.Clientes.AsQueryable();
        if (startDate.HasValue) query = query.Where(c => c.Creado >= startDate.Value);
        if (endDate.HasValue) query = query.Where(c => c.Creado <= endDate.Value);
        
        return await query.CountAsync();
    }

    public async Task<int> GetTotalProductosAsync(DateTime? startDate, DateTime? endDate)
    {
        var query = _context.Productos.AsQueryable();
        if (startDate.HasValue) query = query.Where(p => p.Creado >= startDate.Value);
        if (endDate.HasValue) query = query.Where(p => p.Creado <= endDate.Value);
        
        return await query.CountAsync();
    }

    public async Task<Dictionary<string, int>> GetCitasPorEstadoAsync(DateTime? startDate, DateTime? endDate)
    {
        var query = _context.Citas.AsQueryable();
        if (startDate.HasValue) query = query.Where(c => c.FechaCita >= startDate.Value);
        if (endDate.HasValue) query = query.Where(c => c.FechaCita <= endDate.Value);

        var groups = await query
            .GroupBy(c => c.Estado ?? ResponseMessagesDashboard.NoDefinido)
            .Select(g => new { Estado = g.Key, Count = g.Count() })
            .ToListAsync();

        return groups.ToDictionary(
            x => x.Estado,
            x => x.Count,
            StringComparer.OrdinalIgnoreCase
        );
    }

    public async Task<List<Cita>> GetUltimasCitasAsync(int count)
    {
        return await _context.Citas
            .Include(c => c.Mascota)
                .ThenInclude(m => m.Cliente)
            .Include(c => c.Doctor)
            .OrderByDescending(c => c.FechaCita)
            .ThenByDescending(c => c.HoraCita)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<Producto>> GetProductosBajoStockAsync(int count)
    {
        return await _context.Productos
            .Where(p => p.Stock <= p.StockMinimo)
            .OrderBy(p => p.Stock)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<AlertasInterna>> GetAlertasRecientesAsync(int count)
    {
        return await _context.AlertasInternas
            .OrderByDescending(a => a.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<DashboardEarningByMonthDTO>> GetGananciasMensualesAsync(int meses)
    {
        var startDate = DateTime.UtcNow.AddMonths(-meses);
        
        var facturas = await _context.Facturas
            .Where(f => f.FechaEmision >= startDate && 
                        f.EstadoPago != null && 
                        f.EstadoPago.ToLower() != Status.Pending.ToLower() && 
                        f.EstadoPago.ToLower() != Status.Cancelled.ToLower() && 
                        f.EstadoPago.ToLower() != ResponseMessagesDashboard.Cancelado)
            .Select(f => new { f.FechaEmision, f.Total })
            .ToListAsync();

        // Agrupación en memoria para soportar múltiples bases de datos sin problemas de traducción de LINQ a SQL sobre DateTime
        var result = facturas
            .GroupBy(f => new { f.FechaEmision.Year, f.FechaEmision.Month })
            .Select(g => new DashboardEarningByMonthDTO
            {
                Mes = GetMonthName(g.Key.Month),
                Anio = g.Key.Year,
                Ganancia = g.Sum(f => f.Total)
            })
            .OrderBy(r => r.Anio)
            .ThenBy(r => GetMonthNumber(r.Mes))
            .ToList();

        return result;
    }

    private static string GetMonthName(int month)
    {
        return month switch
        {
            1 => ResponseMessagesDashboard.Months.Ene,
            2 => ResponseMessagesDashboard.Months.Feb,
            3 => ResponseMessagesDashboard.Months.Mar,
            4 => ResponseMessagesDashboard.Months.Abr,
            5 => ResponseMessagesDashboard.Months.May,
            6 => ResponseMessagesDashboard.Months.Jun,
            7 => ResponseMessagesDashboard.Months.Jul,
            8 => ResponseMessagesDashboard.Months.Ago,
            9 => ResponseMessagesDashboard.Months.Sep,
            10 => ResponseMessagesDashboard.Months.Oct,
            11 => ResponseMessagesDashboard.Months.Nov,
            12 => ResponseMessagesDashboard.Months.Dic,
            _ => ResponseMessagesDashboard.UnknownMonth
        };
    }

    private static int GetMonthNumber(string name)
    {
        if (name == ResponseMessagesDashboard.Months.Ene) return 1;
        if (name == ResponseMessagesDashboard.Months.Feb) return 2;
        if (name == ResponseMessagesDashboard.Months.Mar) return 3;
        if (name == ResponseMessagesDashboard.Months.Abr) return 4;
        if (name == ResponseMessagesDashboard.Months.May) return 5;
        if (name == ResponseMessagesDashboard.Months.Jun) return 6;
        if (name == ResponseMessagesDashboard.Months.Jul) return 7;
        if (name == ResponseMessagesDashboard.Months.Ago) return 8;
        if (name == ResponseMessagesDashboard.Months.Sep) return 9;
        if (name == ResponseMessagesDashboard.Months.Oct) return 10;
        if (name == ResponseMessagesDashboard.Months.Nov) return 11;
        if (name == ResponseMessagesDashboard.Months.Dic) return 12;
        return 0;
    }
}
