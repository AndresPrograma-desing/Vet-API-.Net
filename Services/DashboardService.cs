using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Utilities;
using vet_api_Net.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace vet_api_Net.Services;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _repository;
    private readonly ICurrencyService _currencyService;
    private readonly ApiSettingsOptions _apiSettings;

    public DashboardService(IDashboardRepository repository, ICurrencyService currencyService, IOptions<ApiSettingsOptions> apiSettingsOptions)
    {
        _repository = repository;
        _currencyService = currencyService;
        _apiSettings = apiSettingsOptions.Value;
    }

    public async Task<DashboardStatsDTO> GetDashboardStatsAsync(DateTime? startDate, DateTime? endDate, bool useUsd)
    {
        var totalGanancias = await _repository.GetTotalGananciasAsync(startDate, endDate);
        var totalPerdidas = await _repository.GetTotalPerdidasAsync(startDate, endDate);
        var totalFacturas = await _repository.GetTotalFacturasCountAsync(startDate, endDate);
        var totalCitas = await _repository.GetTotalCitasAsync(startDate, endDate);
        var totalMascotas = await _repository.GetTotalMascotasAsync(startDate, endDate);
        var totalClientes = await _repository.GetTotalClientesAsync(startDate, endDate);
        var totalProductos = await _repository.GetTotalProductosAsync(startDate, endDate);

        var citasPorEstado = await _repository.GetCitasPorEstadoAsync(startDate, endDate);
        var ultimasCitasRaw = await _repository.GetUltimasCitasAsync(5);
        var productosBajoStockRaw = await _repository.GetProductosBajoStockAsync(5);
        var alertasRecientesRaw = await _repository.GetAlertasRecientesAsync(5);
        var gananciasMensuales = await _repository.GetGananciasMensualesAsync(6);

        if (useUsd)
        {
            totalGanancias = await _currencyService.ConvertToUsdAsync(totalGanancias);
            totalPerdidas = await _currencyService.ConvertToUsdAsync(totalPerdidas);
            
            foreach (var item in gananciasMensuales)
            {
                item.Ganancia = await _currencyService.ConvertToUsdAsync(item.Ganancia);
            }
        }

        var ultimasCitas = ultimasCitasRaw.Select(c => new DashboardCitaDTO
        {
            Id = c.Id,
            FechaCita = c.FechaCita.ToString(_apiSettings.DateFormat),
            HoraCita = c.HoraCita.ToString(_apiSettings.TimeFormat),
            Motivo = c.Motivo,
            TipoCita = c.TipoCita ?? string.Empty,
            Estado = c.Estado ?? string.Empty,
            MascotaNombre = c.Mascota?.Nombre ?? string.Empty,
            ClienteNombre = c.Mascota?.Cliente != null 
                ? $"{c.Mascota.Cliente.Nombre} {c.Mascota.Cliente.Apellido}".Trim() 
                : string.Empty,
            DoctorNombre = c.Doctor != null 
                ? $"{c.Doctor.Nombre} {c.Doctor.Apellido}".Trim() 
                : string.Empty
        }).ToList();

        var productosBajoStock = productosBajoStockRaw.Select(p => new DashboardProductDTO
        {
            Id = p.Id,
            Codigo = p.Codigo,
            Nombre = p.Nombre,
            Stock = p.Stock ?? 0,
            StockMinimo = p.StockMinimo ?? 0,
            PrecioVenta = p.PrecioVenta
        }).ToList();

        var alertasRecientes = alertasRecientesRaw.Select(a => new DashboardAlertDTO
        {
            Id = a.Id,
            Titulo = a.Titulo,
            Mensaje = a.Mensaje ?? string.Empty,
            Tipo = a.Tipo ?? string.Empty,
            Prioridad = a.Prioridad ?? string.Empty,
            Fecha = a.CreatedAt
        }).ToList();

        return new DashboardStatsDTO
        {
            TotalGanancias = totalGanancias,
            TotalPerdidas = totalPerdidas,
            TotalFacturas = totalFacturas,
            TotalCitas = totalCitas,
            TotalMascotas = totalMascotas,
            TotalClientes = totalClientes,
            TotalProductos = totalProductos,
            CitasPorEstado = citasPorEstado,
            UltimasCitas = ultimasCitas,
            ProductosBajoStock = productosBajoStock,
            AlertasRecientes = alertasRecientes,
            GananciasMensuales = gananciasMensuales
        };
    }
}
