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
    public class PetsRepository : IPetsRepository
    {
        private readonly AppDbContext _context;

        public PetsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Mascota>> GetAllMascotasWithClienteAsync()
        {
            return await _context.Mascotas
                .Include(m => m.Cliente)
                .ToListAsync();
        }

        public async Task<Mascota?> GetMascotaByIdWithClienteAsync(int id)
        {
            return await _context.Mascotas
                .Include(m => m.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<List<Mascota>> GetMascotasByClienteNameAsync(string nombre)
        {
            var lower = nombre.Trim().ToLowerInvariant();
            return await _context.Mascotas
                .Include(m => m.Cliente)
                .Where(m => m.Cliente != null && m.Cliente.Nombre.ToLower() == lower)
                .ToListAsync();
        }
    }
}
