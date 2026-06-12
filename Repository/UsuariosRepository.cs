using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Models;
using vet_api_Net.Interfaces.Repositories;

namespace vet_api_Net.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly AppDbContext _context;

    public UsersRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> GetByEmailAndRolAsync(string email, string rol)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email && u.Rol == rol);
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Usuario?> GetByIdAsync(int id)
    {
        return await _context.Usuarios.FindAsync(id);
    }

    public void Update(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> IsUserDisabledAsync(string email)
    {
        var user = await _context.Usuarios
            .Where(u => u.Email == email)
            .Select(u => new { u.Activo }) 
            .FirstOrDefaultAsync();

        if (user == null) return false;

        return user.Activo == null || user.Activo == false;
    }

    public async Task<List<Usuario>> GetAllOrderedByIdAsync()
    {
        return await _context.Usuarios.OrderBy(u => u.Id).ToListAsync();
    }

    public async Task<List<Usuario>> GetSecretariasOrderedByIdAsync()
    {
        return await _context.Usuarios
            .Where(u => u.Rol != null && u.Rol.ToLower() == "secretaria")
            .OrderBy(u => u.Id)
            .ToListAsync();
    }

    public void Add(Usuario user)
    {
        _context.Usuarios.Add(user);
    }

    public void Delete(Usuario user)
    {
        _context.Usuarios.Remove(user);
    }
}