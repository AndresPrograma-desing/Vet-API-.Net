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

    public async Task<int> GetTotalCitasAsync(DateTime? startDate, DateTime? endDate, string? status)
    {
        var query = _context.Citas.AsQueryable();
        if (startDate.HasValue) query = query.Where(c => c.FechaCita >= startDate.Value);
        if (endDate.HasValue) query = query.Where(c => c.FechaCita <= endDate.Value);
        if (!string.IsNullOrWhiteSpace(status)) query = query.Where(c => c.Estado != null && c.Estado.ToLower() == status.ToLower());

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

    public async Task<Dictionary<string, int>> GetCitasPorEstadoAsync(DateTime? startDate, DateTime? endDate, string? status)
    {
        var query = _context.Citas.AsQueryable();
        if (startDate.HasValue) query = query.Where(c => c.FechaCita >= startDate.Value);
        if (endDate.HasValue) query = query.Where(c => c.FechaCita <= endDate.Value);
        if (!string.IsNullOrWhiteSpace(status)) query = query.Where(c => c.Estado != null && c.Estado.ToLower() == status.ToLower());

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

    public async Task<List<Cita>> GetUltimasCitasAsync(int count, string? status)
    {
        var query = _context.Citas
            .Include(c => c.Mascota)
                .ThenInclude(m => m.Cliente)
            .Include(c => c.Doctor)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status)) query = query.Where(c => c.Estado != null && c.Estado.ToLower() == status.ToLower());

        return await query
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

    public async Task<List<DashboardEarningByMonthDTO>> GetGroupedEarningsAsync(DateTime? startDate, DateTime? endDate, string groupBy)
    {
        var effectiveEnd = endDate ?? DateTime.Now;
        var effectiveStart = startDate ?? groupBy switch
        {
            ResponseMessagesDashboard.GroupBy.Day => effectiveEnd.AddDays(-30),
            ResponseMessagesDashboard.GroupBy.Week => effectiveEnd.AddDays(-7 * 12),
            ResponseMessagesDashboard.GroupBy.Year => effectiveEnd.AddYears(-5),
            _ => effectiveEnd.AddMonths(-6)
        };

        var facturas = await _context.Facturas
            .Where(f => f.FechaEmision >= effectiveStart &&
                        f.FechaEmision <= effectiveEnd &&
                        f.EstadoPago != null &&
                        f.EstadoPago.ToLower() != Status.Pending.ToLower() &&
                        f.EstadoPago.ToLower() != Status.Cancelled.ToLower() &&
                        f.EstadoPago.ToLower() != ResponseMessagesDashboard.Cancelado)
            .Select(f => new { f.FechaEmision, f.Total })
            .ToListAsync();

        // Agrupación en memoria para soportar múltiples bases de datos sin problemas de traducción de LINQ a SQL sobre DateTime
        Func<DateTime, DateTime> bucketStart = groupBy switch
        {
            ResponseMessagesDashboard.GroupBy.Day => d => d.Date,
            ResponseMessagesDashboard.GroupBy.Week => d => StartOfWeek(d),
            ResponseMessagesDashboard.GroupBy.Year => d => new DateTime(d.Year, 1, 1),
            _ => d => new DateTime(d.Year, d.Month, 1)
        };

        var result = facturas
            .GroupBy(f => bucketStart(f.FechaEmision))
            .Select(g => new DashboardEarningByMonthDTO
            {
                FechaInicio = g.Key,
                Periodo = FormatPeriodLabel(g.Key, groupBy),
                Mes = groupBy == ResponseMessagesDashboard.GroupBy.Month ? GetMonthName(g.Key.Month) : string.Empty,
                Anio = g.Key.Year,
                Ganancia = g.Sum(f => f.Total)
            })
            .OrderBy(r => r.FechaInicio)
            .ToList();

        return result;
    }

    private static DateTime StartOfWeek(DateTime date)
    {
        int diff = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        return date.Date.AddDays(-diff);
    }

    private static string FormatPeriodLabel(DateTime bucketStart, string groupBy)
    {
        return groupBy switch
        {
            ResponseMessagesDashboard.GroupBy.Day => bucketStart.ToString("dd/MM/yyyy"),
            ResponseMessagesDashboard.GroupBy.Week => $"Semana del {bucketStart:dd/MM/yyyy}",
            ResponseMessagesDashboard.GroupBy.Year => bucketStart.Year.ToString(),
            _ => $"{GetMonthName(bucketStart.Month)} {bucketStart.Year}"
        };
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
}
