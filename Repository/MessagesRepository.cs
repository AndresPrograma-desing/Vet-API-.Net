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
    public class MessagesRepository : IMessagesRepository
    {
        private readonly AppDbContext _context;

        public MessagesRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Mensaje>> GetConversationAsync(int userId, int otherUserId)
        {
            return await _context.Mensajes
                .Where(m => (m.EmisorId == userId && m.ReceptorId == otherUserId) || (m.EmisorId == otherUserId && m.ReceptorId == userId))
                .OrderBy(m => m.FechaEnvio)
                .ToListAsync();
        }

        public async Task<List<Mensaje>> GetUserMessagesAsync(int userId)
        {
            return await _context.Mensajes
                .Where(m => m.ReceptorId == userId)
                .OrderByDescending(m => m.FechaEnvio)
                .ToListAsync();
        }

        public async Task<Mensaje?> GetByIdAsync(int messageId)
        {
            return await _context.Mensajes.FindAsync(messageId);
        }

        public void Add(Mensaje mensaje)
        {
            _context.Mensajes.Add(mensaje);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Mensaje>> GetUnreadMessagesAsync()
        {
            return await _context.Mensajes
                .Where(m => m.Leido == false)
                .OrderBy(m => m.FechaEnvio)
                .ToListAsync();
        }
    }
}
