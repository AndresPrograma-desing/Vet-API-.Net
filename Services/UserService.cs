using System.Linq;
using vet_api_Net.Data;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Models;
using DTOs;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Constants;

namespace vet_api_Net.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Usuario>> GetAllUserAsync()
    {
        var query = _context.Usuarios.AsQueryable();

        var rows = await query.OrderBy(u => u.Id)
                              .ToListAsync();

 
        rows.ForEach(u => u.Password = string.Empty);
        return rows;
    }

    public async Task<Usuario?> VerifyCredentialsAsync(string email, string password)
    {
         if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(password))
            return null;

        var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return null;

        var stored = user.Password ?? string.Empty;
        bool valid = false;
 
        if (stored.StartsWith("$2a$") || stored.StartsWith("$2b$") || stored.StartsWith("$2y$") || stored.StartsWith("$2x$"))
        {
            try
            {
                valid = BCrypt.Net.BCrypt.Verify(password, stored);
            }
            catch
            {
                valid = false;
            }
        }
        else
        {
 
            if (string.Equals(password, stored, StringComparison.Ordinal))
            {
                valid = true;
                try
                {
                    var newHash = BCrypt.Net.BCrypt.HashPassword(password);
                    user.Password = newHash;
                    _context.Usuarios.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch { }
            }
        }

        return valid ? user : null;
    }

    public async Task<Usuario> CreateUserAsync(CreateUserDTO userDto)
    {
        var user = new Usuario
        {
            Nombre = userDto.Nombre,
            Apellido = userDto.Apellido,
            Email = userDto.Email,
            Password = string.IsNullOrWhiteSpace(userDto.Password) ? string.Empty : BCrypt.Net.BCrypt.HashPassword(userDto.Password),
            Rol = userDto.Rol,
            Activo = true
        };

        _context.Usuarios.Add(user);
        await _context.SaveChangesAsync();
 
        user.Password = string.Empty;
        return user;
    }

    public async Task<List<Usuario>> GetSecretariasAsync()
    {
        var query = _context.Usuarios
            .Where(u => u.Rol != null && u.Rol.ToLower() == "secretaria")
            .OrderBy(u => u.Id);

        var rows = await query.ToListAsync();
        rows.ForEach(u => u.Password = string.Empty);
        return rows;
    }

public async Task<Usuario?> UpdateUserStatusAsync(int id, bool status)
{
    var user = await _context.Usuarios.FindAsync(id);
    if (user == null) return null;

    user.Activo = status;
    
    await _context.SaveChangesAsync();

    user.Password = string.Empty; 
    return user;
}

public async Task<Usuario?> DisableUserAsync(int id) => await UpdateUserStatusAsync(id, false);
public async Task<Usuario?> EnableUserAsync(int id) => await UpdateUserStatusAsync(id, true);

public async Task<Usuario?> DeleteUserAsync(int id)
    {
        var user = await _context.Usuarios.FindAsync(id);
        if (user == null) return null;

        _context.Usuarios.Remove(user);
        await _context.SaveChangesAsync();

        user.Password = string.Empty; 
        return user;
    }
    public async Task<string?> UserStatusAsync(int id)
    {
        var user = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return null;
        
     return (user.Activo ?? false) ? ResponseMessagesUsers.UsersVariable.UserStatusActivo : ResponseMessagesUsers.UsersVariable.UserStatusInactivo;
    }
}
    