using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Models;
using vet_api_Net.Interfaze.Repositories;

namespace vet_api_Net.Repositories;

public class PasswordResetTicketRepository : IPasswordResetTicketRepository
{
    private readonly AppDbContext _context;

    public PasswordResetTicketRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PasswordResetTicket?> GetPendingTicketAsync(int userId)
    {
        return await _context.PasswordResetTickets
            .Where(t => t.UsuarioId == userId && t.Estado == "Pending" && t.Expiracion > DateTime.Now)
            .FirstOrDefaultAsync();
    }

    public Task AddTicketAsync(PasswordResetTicket ticket)
    {
        _context.PasswordResetTickets.Add(ticket);
        return Task.CompletedTask;
    }

    public async Task<PasswordResetTicket?> GetTicketByTokenAsync(string token)
    {
        return await _context.PasswordResetTickets
            .Include(t => t.Usuario)
            .FirstOrDefaultAsync(t => t.Token == token);
    }

    public async Task<PasswordResetTicket?> GetLatestTicketByUserIdAsync(int userId)
    {
        return await _context.PasswordResetTickets
            .Where(t => t.UsuarioId == userId)
            .OrderByDescending(t => t.Creado)
            .FirstOrDefaultAsync();
    }

    public async Task<List<PasswordResetTicket>> GetPendingAndAcceptedTicketsAsync()
    {
        return await _context.PasswordResetTickets
            .Include(t => t.Usuario)
            .Where(t => t.Estado == "Pending" || t.Estado == "Accepted")
            .OrderByDescending(t => t.Creado)
            .ToListAsync();
    }

    public async Task<PasswordResetTicket?> GetAcceptedTicketByUserIdAsync(int userId)
    {
        return await _context.PasswordResetTickets
            .Where(t => t.UsuarioId == userId && t.Estado == "Accepted")
            .OrderByDescending(t => t.Creado)
            .FirstOrDefaultAsync();
    }

    public Task UpdateTicketAsync(PasswordResetTicket ticket)
    {
        _context.PasswordResetTickets.Update(ticket);
        return Task.CompletedTask;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
