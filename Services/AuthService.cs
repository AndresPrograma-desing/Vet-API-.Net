using System;
using System.Linq;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using vet_api_Net.Models;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.DTOs;
using DTOs;

namespace vet_api_Net.Services;

public class AuthService : IAuthService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUsersRepository usuariosRepository, IConfiguration configuration)
    {
        _usersRepository = usuariosRepository;
        _configuration = configuration;
    }

    public async Task<Usuario?> LoginAsync(LoginRequest request)
    {
        ArgumentNullException.ThrowIfNull(request); 
        var (isValid, message) = await VerifyEmailRoleAsync(request.Email, request.Rol);
        if (!isValid)
        {
            throw new InvalidOperationException(message);
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return null;
        }

        var user = await _usersRepository.GetByEmailAndRolAsync(request.Email, request.Rol);
        if (user == null) throw new InvalidOperationException(message);

        if (await _usersRepository.IsUserDisabledAsync(request.Email))
            throw new InvalidOperationException(ResponseMessagesLogin.IsDisabled);

        var stored = user.Password ?? string.Empty;
        bool valid = false;

        if (stored.StartsWith("$2a$") || stored.StartsWith("$2b$") || stored.StartsWith("$2y$") || stored.StartsWith("$2x$"))
        {
            try { valid = BCrypt.Net.BCrypt.Verify(request.Password, stored); } catch { valid = false; }
        }
        else
        {
            if (string.Equals(request.Password, stored, StringComparison.Ordinal))
            {
                valid = true;
                user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
                _usersRepository.Update(user);
                await _usersRepository.SaveChangesAsync();
            }
        }

        if (!valid) return null;

        return user;
    }

    public string GenerateToken(Usuario user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var key = _configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(key)) throw new InvalidOperationException("JWT key is not configured.");

        var tokenHandler = new JwtSecurityTokenHandler();
        var keyBytes = Encoding.UTF8.GetBytes(key);

        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Rol ?? string.Empty)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<Usuario?> UpdatePasswordAsync(UpdatePasswordRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Email)) throw new ArgumentException(ResponseMessagesLogin.RequiredEmail);
        if (string.IsNullOrWhiteSpace(request.OldPassword)) throw new ArgumentException(ResponseMessagesLogin.RequiredOldPassword);
        if (string.IsNullOrWhiteSpace(request.NewPassword)) throw new ArgumentException(ResponseMessagesLogin.RequiredPassword);
        if (request.NewPassword.Length < 6) throw new ArgumentException(ResponseMessagesLogin.IsMenorThan6);
        if (request.NewPassword.Any(char.IsWhiteSpace)) throw new ArgumentException(ResponseMessagesLogin.NoWhiteSpace);

        var user = await _usersRepository.GetByEmailAsync(request.Email);
        if (user == null) throw new KeyNotFoundException(ResponseMessagesUsers.UserNotFound);

        if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.Password))
            throw new ArgumentException(ResponseMessagesLogin.IncorrectOldPassword);

        bool isSamePassword = false;
        try
        {
            if (!string.IsNullOrEmpty(user.Password))
                isSamePassword = BCrypt.Net.BCrypt.Verify(request.NewPassword, user.Password);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            isSamePassword = (request.NewPassword == user.Password);
        }

        if (isSamePassword) throw new InvalidOperationException(ResponseMessagesLogin.IsEqualToCurrentPassword);

        try
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            if (user.PasswordRecoveryCode == ResponseMessagesPasswordRecovery.RequirePasswordChangeCode)
            {
                user.PasswordRecoveryCode = null;
            }
            _usersRepository.Update(user);
            await _usersRepository.SaveChangesAsync();
            return user;
        }
        catch (Exception)
        {
            throw new Exception(ResponseMessagesLogin.ErrorSavingPassword);
        }
    }

    public async Task<Usuario?> GetUserByIdAsync(int id)
    {
        return await _usersRepository.GetByIdAsync(id);
    }

    private async Task<(bool IsValid, string Message)> VerifyEmailRoleAsync(string email, string roleName)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(roleName))
        {
            return (false, ResponseMessagesLogin.EmailAndRolRequired);
        }

        var userRole = await _usersRepository.GetRoleByEmailAsync(email);

        if (userRole == null)
        {
            return (false, ResponseMessagesLogin.UserNotExisting);
        }

        if (userRole != roleName)
        {
            return (false, ResponseMessagesLogin.NoRolForUser);
        }

        return (true, "");
    }
}
