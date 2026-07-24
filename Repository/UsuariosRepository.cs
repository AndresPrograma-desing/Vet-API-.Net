using System.Threading.Tasks;
using DTOs;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Models;

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

    public async Task<Usuario?> GetByNameAndApellidoAsync(string nombre, string apellido)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Nombre == nombre && u.Apellido == apellido);
    }

    public async Task<Usuario?> GetByIdAsync(int id)
    {
        var users = await _context.Usuarios
        .Where(u => u.Rol != "assistant")
        .Where(u => u.Id == id)
        .FirstOrDefaultAsync();

        return users;
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

    public async Task<string?> GetRoleByEmailAsync(string email)
    {
        return await _context.Usuarios
            .AsNoTracking()
            .Where(u => u.Email == email)
            .Select(u => u.Rol)
            .FirstOrDefaultAsync();
    }

    public async Task<ChangeNameUsersDTO?> ChangeNameUsersAsync(int id, string newName, string newLastName, string newEmail, string newPhone)
    {
        var user = await _context.Usuarios.FindAsync(id);
        if (user == null) return null;

        string cleanName = (newName ?? "").Trim();
        string cleanLastName = (newLastName ?? "").Trim();
        string cleanEmail = (newEmail ?? "").Trim();
        string cleanPhone = (newPhone ?? "").Trim();

        if (string.IsNullOrEmpty(cleanName)) cleanName = user.Nombre;
        if (string.IsNullOrEmpty(cleanLastName)) cleanLastName = user.Apellido;
        if (string.IsNullOrEmpty(cleanEmail)) cleanEmail = user.Email;
        if (string.IsNullOrEmpty(cleanPhone)) cleanPhone = user.Telefono;

        bool hasChanges = user.Nombre != cleanName ||
                          user.Apellido != cleanLastName ||
                          user.Email != cleanEmail ||
                          user.Telefono != cleanPhone;

        if (hasChanges)
        {
            user.Nombre = cleanName;
            user.Apellido = cleanLastName;
            user.Email = cleanEmail;
            user.Telefono = cleanPhone;
            user.Actualizado = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        return new ChangeNameUsersDTO
        {
            IdUser = user.Id,
            NewUserName = user.Nombre,
            NewLastName = user.Apellido,
            NewEmail = user.Email,
            NewPhone = user.Telefono
        };
    }

    public async Task<List<Usuario>> GetAllUsersAsync()
    {
        return await _context.Usuarios
            .Where(u => u.Rol != "assistant")
            .OrderBy(u => u.Id)
            .ToListAsync();
    }

    public async Task<List<Usuario>> GetUsersByRoleAsync(string role)
    {
        return await _context.Usuarios
            .Where(u => u.Rol != null && u.Rol.ToLower() == role.ToLower())
            .OrderBy(u => u.Id)
            .ToListAsync();
    }

    public Task AddUserAsync(Usuario user)
    {
        _context.Usuarios.Add(user);
        return Task.CompletedTask;
    }

    public Task DeleteUserAsync(Usuario user)
    {
        _context.Usuarios.Remove(user);
        return Task.CompletedTask;
    }

    public async Task<Usuario?> SaveAvatarUrl(int userId, string avatarUrl)
    {
        var user = await _context.Usuarios.FindAsync(userId);
        if (user == null) return null;

        user.AvatarUrl = avatarUrl;
        user.Actualizado = DateTime.Now;
        await _context.SaveChangesAsync();
        return user;
    }
}