using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vet_api_Net.Models;
using DTOs;
using vet_api_Net.Interfaces.Services;
using vet_api_Net.Interfaces.Repositories;
using vet_api_Net.Constants;

namespace vet_api_Net.Services
{
    public class UserService : IUserService
    {
        private readonly IUsersRepository _usersRepository;

        public UserService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public async Task<List<Usuario>> GetAllUserAsync()
        {
            var rows = await _usersRepository.GetAllOrderedByIdAsync();
            rows.ForEach(u => u.Password = string.Empty);
            return rows;
        }

        public async Task<Usuario?> VerifyCredentialsAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(password))
                return null;

            var user = await _usersRepository.GetByEmailAsync(email);
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
                        _usersRepository.Update(user);
                        await _usersRepository.SaveChangesAsync();
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

            _usersRepository.Add(user);
            await _usersRepository.SaveChangesAsync();

            user.Password = string.Empty;
            return user;
        }

        public async Task<List<Usuario>> GetSecretariasAsync()
        {
            var rows = await _usersRepository.GetSecretariasOrderedByIdAsync();
            rows.ForEach(u => u.Password = string.Empty);
            return rows;
        }

        public async Task<Usuario?> UpdateUserStatusAsync(int id, bool status)
        {
            var user = await _usersRepository.GetByIdAsync(id);
            if (user == null) return null;

            user.Activo = status;
            await _usersRepository.SaveChangesAsync();

            user.Password = string.Empty;
            return user;
        }

        public async Task<Usuario?> DisableUserAsync(int id) => await UpdateUserStatusAsync(id, false);
        public async Task<Usuario?> EnableUserAsync(int id) => await UpdateUserStatusAsync(id, true);

        public async Task<Usuario?> DeleteUserAsync(int id)
        {
            var user = await _usersRepository.GetByIdAsync(id);
            if (user == null) return null;

            _usersRepository.Delete(user);
            await _usersRepository.SaveChangesAsync();

            user.Password = string.Empty;
            return user;
        }

        public async Task<string?> UserStatusAsync(int id)
        {
            var user = await _usersRepository.GetByIdAsync(id);
            if (user == null) return null;

            return (user.Activo ?? false) ? ResponseMessagesUsers.UsersVariable.UserStatusActivo : ResponseMessagesUsers.UsersVariable.UserStatusInactivo;
        }
    }
}