using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Models;
using vet_api_Net.DTOs;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Extensions;

namespace vet_api_Net.Repositories;

public class AlertsRepository : IAlertsRepository
{
    private readonly AppDbContext _context;

    public AlertsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AlertNotificationDTO>> GetByDestinationAsync(string destination)
    {
        var limiteTiempo = DateTime.Now.AddMinutes(-1); 

        return await _context.AlertasInternas
            .WhereEqualsIgnoreCase(n => n.DestinatarioRol, destination)
            .Where(n => n.Leida == false || (n.Leida == true && n.FechaLectura > limiteTiempo)) 
            .Select(n => new AlertNotificationDTO
            {
                Id = n.Id,
                Titulo = n.Titulo,
                Mensaje = n.Mensaje ?? string.Empty,
                Tipo = n.Tipo,
                Prioridad = n.Prioridad ?? "0",
                Destino = n.DestinatarioRol ?? string.Empty,
                Origen = n.ReferenciaTabla ?? string.Empty,
                Completada = n.Completada ?? false,
                Leida = n.Leida ?? false,
                FechaLectura = n.FechaLectura,
                CreatedAt = n.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<bool> AnyRecentDuplicateAsync(string title, System.DateTime threshold)
    {
        return await _context.AlertasInternas.AnyAsync(n => n.Titulo == title && n.CreatedAt > threshold);
    }

    public void Add(AlertasInterna alert)
    {
        _context.AlertasInternas.Add(alert);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> MarkAsReadAsync(int alertId)
    {
        var alert = await _context.AlertasInternas.FindAsync(alertId);
        if (alert == null) return false;

        alert.Leida = true;
        alert.FechaLectura = DateTime.Now;
        return await SaveChangesAsync();
    }

    public async Task<int> ExecutePurgeReadAlertsAsync()
    {
        return await _context.AlertasInternas
            .Where(n => n.Leida == true)
            .ExecuteDeleteAsync();
    }
}