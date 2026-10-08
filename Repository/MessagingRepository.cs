using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Constants;
using vet_api_Net.Data;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Models;

//Describe: Acceso a datos de los mensajes internos entre usuarios.
namespace vet_api_Net.Repositories;

public class MessagingRepository : IMessagingRepository
{
    private readonly AppDbContext _context;

    public MessagingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Mensaje?> GetByIdAsync(int id)
        => await _context.Mensajes.FindAsync(id);

    public async Task<List<Mensaje>> GetConversationAsync(int userId, int otherUserId)
        => await _context.Mensajes
            .Where(m => (m.EmisorId == userId && m.ReceptorId == otherUserId) || (m.EmisorId == otherUserId && m.ReceptorId == userId))
            .OrderBy(m => m.FechaEnvio)
            .ToListAsync();

    public async Task<List<Mensaje>> GetRecentConversationAsync(int userId, int otherUserId, int take)
        => await _context.Mensajes
            .Where(m => (m.EmisorId == userId && m.ReceptorId == otherUserId) || (m.EmisorId == otherUserId && m.ReceptorId == userId))
            .OrderBy(m => m.FechaEnvio)
            .Take(take)
            .ToListAsync();

    public async Task<List<Mensaje>> GetByReceptorAsync(int userId)
        => await _context.Mensajes
            .Where(m => m.ReceptorId == userId)
            .OrderByDescending(m => m.FechaEnvio)
            .ToListAsync();

    public async Task<List<Mensaje>> GetUnreadAsync(CancellationToken cancellationToken = default)
        => await _context.Mensajes
            .Where(m => m.Leido == false)
            .OrderBy(m => m.FechaEnvio)
            .ToListAsync(cancellationToken);

    public async Task<bool> TableExistsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync(cancellationToken);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = MessagePoller.Query;
            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            return result != null && result != DBNull.Value;
        }
        catch
        {
            return false;
        }
    }

    public async Task AddAsync(Mensaje mensaje)
        => await _context.Mensajes.AddAsync(mensaje);

    public async Task<bool> SaveChangesAsync()
        => await _context.SaveChangesAsync() > 0;
}
