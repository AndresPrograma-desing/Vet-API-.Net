using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Models;

namespace vet_api_Net.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly AppDbContext _context;

    public ClientRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Cliente?> GetByIdSimpleAsync(int id)
    {
        return await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Cliente>> GetAllWithDetailsAsync()
    {
        return await _context.Clientes
            .Include(c => c.Mascota)
                .ThenInclude(m => m.Consulta)
            .Include(c => c.Mascota)
                .ThenInclude(m => m.Cita)
            .ToListAsync();
    }
    public async Task<Cliente?> DeleteClientAsync(int id)
    {
        var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
        if (cliente == null) return null;

        var mascotaIds = await _context.Mascotas
            .Where(m => m.ClienteId == id)
            .Select(m => m.Id)
            .ToListAsync();

        if (mascotaIds.Any())
        {
            var consultaIds = await _context.Consultas
                .Where(c => mascotaIds.Contains(c.MascotaId))
                .Select(c => c.Id)
                .ToListAsync();

            if (consultaIds.Any())
            {
                await _context.ConsultasProductos
                    .Where(cp => consultaIds.Contains(cp.ConsultaId))
                    .ExecuteDeleteAsync();
            }

            var facturaIds = await _context.Facturas
                .Where(f => f.MascotaId != null && mascotaIds.Contains(f.MascotaId.Value))
                .Select(f => f.Id)
                .ToListAsync();

            if (facturaIds.Any())
            {
                await _context.DetallesFacturas
                    .Where(df => facturaIds.Contains(df.FacturaId))
                    .ExecuteDeleteAsync();
            }

            await _context.Facturas
                .Where(f => f.MascotaId != null && mascotaIds.Contains(f.MascotaId.Value))
                .ExecuteDeleteAsync();

            await _context.Vacunas
                .Where(v => mascotaIds.Contains(v.MascotaId))
                .ExecuteDeleteAsync();

            await _context.HistoriasClinicas
                .Where(h => mascotaIds.Contains(h.MascotaId))
                .ExecuteDeleteAsync();

            await _context.Citas
                .Where(c => mascotaIds.Contains(c.MascotaId))
                .ExecuteDeleteAsync();

            await _context.Consultas
                .Where(c => mascotaIds.Contains(c.MascotaId))
                .ExecuteDeleteAsync();

            await _context.Mascotas
                .Where(m => m.ClienteId == id)
                .ExecuteDeleteAsync();
        }

        // Delete client direct facturas details if any
        var clientFacturaIds = await _context.Facturas
            .Where(f => f.ClienteId == id)
            .Select(f => f.Id)
            .ToListAsync();

        if (clientFacturaIds.Any())
        {
            await _context.DetallesFacturas
                .Where(df => clientFacturaIds.Contains(df.FacturaId))
                .ExecuteDeleteAsync();

            await _context.Facturas
                .Where(f => f.ClienteId == id)
                .ExecuteDeleteAsync();
        }

        await _context.Clientes
            .Where(c => c.Id == id)
            .ExecuteDeleteAsync();

        return cliente;
    }

    public async Task<Cliente> UpdateClientAsync(Cliente client)
    {
        _context.Clientes.Update(client);
        await _context.SaveChangesAsync();
        return client;
    }
}