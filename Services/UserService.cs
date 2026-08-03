using System.Linq;
using DTOs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using vet_api_Net.Constants;
using vet_api_Net.Data;
using vet_api_Net.Hubs;
using vet_api_Net.Infrastructure.Configuration;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Models;

namespace vet_api_Net.Services;

public class UserService : IUserService
{
    private readonly IPasswordResetTicketRepository _ticketRepository;
    private readonly IUsersRepository _userRepository;
    private readonly IEmailSenderService _emailSenderService;
    private readonly IHubContext<NotificactionsPush> _hubContext;
    private readonly IEmailTemplateRepository _templateRepository;
    private readonly TemplatesHTML _templatesHTML;
    private readonly ApiSettingsOptions _apiSettings;

    public UserService(
        IPasswordResetTicketRepository ticketRepository,
        IUsersRepository userRepository,
        IEmailSenderService emailSenderService,
        IHubContext<NotificactionsPush> hubContext,
        IEmailTemplateRepository templateRepository,
        IOptions<TemplatesHTML> templatesHTML,
        IOptions<ApiSettingsOptions> apiSettings)
    {
        _ticketRepository = ticketRepository;
        _userRepository = userRepository;
        _emailSenderService = emailSenderService;
        _hubContext = hubContext;
        _templateRepository = templateRepository;
        _templatesHTML = templatesHTML.Value;
        _apiSettings = apiSettings.Value;
    }

    public async Task<List<Usuario>> GetAllUserAsync()
    {
        var rows = await _userRepository.GetAllUsersAsync();


        rows.ForEach(u => u.Password = string.Empty);
        return rows;
    }

    public async Task<Usuario?> VerifyCredentialsAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(password))
            return null;

        var user = await _userRepository.GetByEmailAsync(email);
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
                    _userRepository.Update(user);
                    await _userRepository.SaveChangesAsync();
                }
                catch { }
            }
        }

        return valid ? user : null;
    }

    public async Task<Usuario> CreateUserAsync(CreateUserDTO userDto)
    {
        if (userDto == null)
        {
            throw new ArgumentNullException(nameof(userDto));
        }

        if (string.IsNullOrWhiteSpace(userDto.Email))
        {
            throw new ArgumentException(ResponseMessagesUsers.EmailRequired);
        }

        if (string.IsNullOrWhiteSpace(userDto.Nombre) || string.IsNullOrWhiteSpace(userDto.Apellido))
        {
            throw new ArgumentException(ResponseMessagesUsers.NameRequired);
        }

        var existingUserByEmail = await _userRepository.GetByEmailAsync(userDto.Email);
        if (existingUserByEmail != null)
        {
            throw new InvalidOperationException(ResponseMessagesUsers.ExistingEmail);
        }

        var existingUserByName = await _userRepository.GetByNameAndApellidoAsync(userDto.Nombre, userDto.Apellido);
        if (existingUserByName != null)
        {
            throw new InvalidOperationException(ResponseMessagesUsers.ExistingUsername);
        }

        var user = new Usuario
        {
            Nombre = userDto.Nombre.Trim(),
            Apellido = userDto.Apellido.Trim(),
            Email = userDto.Email.Trim(),
            Telefono = userDto.Telefono?.Trim() ?? string.Empty,
            Password = string.IsNullOrWhiteSpace(userDto.Password) ? string.Empty : BCrypt.Net.BCrypt.HashPassword(userDto.Password),
            Rol = userDto.Rol,
            Activo = true,
            Creado = DateTime.Now,
            Actualizado = DateTime.Now
        };

        try
        {
            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            var detailMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            throw new InvalidOperationException(detailMessage, ex);
        }
        catch (Exception ex)
        {
            var detailMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            throw new Exception(detailMessage, ex);
        }

        user.Password = string.Empty;
        return user;
    }

    public async Task<List<Usuario>> GetSecretariasAsync()
    {
        var rows = await _userRepository.GetUsersByRoleAsync("secretaria");
        rows.ForEach(u => u.Password = string.Empty);
        return rows;
    }

    public async Task<Usuario?> UpdateUserStatusAsync(int id, bool status)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return null;

        user.Activo = status;

        await _userRepository.SaveChangesAsync();

        user.Password = string.Empty;
        return user;
    }

    public async Task<Usuario?> DisableUserAsync(int id) => await UpdateUserStatusAsync(id, false);
    public async Task<Usuario?> EnableUserAsync(int id) => await UpdateUserStatusAsync(id, true);

    public async Task<Usuario?> DeleteUserAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return null;

        if (string.Equals(user.Rol, "admin", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(ResponseMessagesUsers.CannotDeleteAdmin);
        }

        try
        {
            await _userRepository.DeleteUserAsync(user);
            await _userRepository.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException(ResponseMessagesUsers.CannotDeleteUserWithDependencies, ex);
        }
        catch (Exception ex)
        {
            var detailMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            throw new Exception(detailMessage, ex);
        }

        user.Password = string.Empty;
        return user;
    }
    public async Task<string?> UserStatusAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return null;

        return (user.Activo ?? false) ? ResponseMessagesUsers.UsersVariable.UserStatusActivo : ResponseMessagesUsers.UsersVariable.UserStatusInactivo;
    }

    public async Task<bool> RequestPasswordResetAsync(string email, string resetLinkBase)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null) return false;

        var existingTicket = await _ticketRepository.GetPendingTicketAsync(user.Id);

        if (existingTicket != null)
        {
            throw new InvalidOperationException(ResponseMessagesUsers.PasswordResetAlreadyPending);
        }

        user.Activo = false;
        _userRepository.Update(user);

        var expirationMinutes = _apiSettings.PasswordRecoveryExpirationMinutes > 0
            ? _apiSettings.PasswordRecoveryExpirationMinutes
            : 15;

        var ticket = new PasswordResetTicket
        {
            UsuarioId = user.Id,
            Token = Guid.NewGuid().ToString("N"),
            Estado = "Pending",
            Expiracion = DateTime.Now.AddMinutes(expirationMinutes),
            Creado = DateTime.Now
        };

        await _ticketRepository.AddTicketAsync(ticket);
        await _userRepository.SaveChangesAsync();
        await _ticketRepository.SaveChangesAsync();

        string confirmLink = $"{resetLinkBase}?token={ticket.Token}";

        var template = await _templateRepository.GetTemplateByTypeAsync(_templatesHTML.ConfirmChangePass ?? string.Empty);
        if (template == null) throw new InvalidOperationException("Template HTML no encontrado.");

        string htmlBody = template.HtmlCode;
        htmlBody = htmlBody.Replace("{{usuario}}", $"{user.Nombre} {user.Apellido}".Trim());
        htmlBody = htmlBody.Replace("{{confirm_link}}", confirmLink);
        htmlBody = htmlBody.Replace("{{system_name}}", _apiSettings.SystemName);

        var sent = await _emailSenderService.SendEmailAsync(user.Email, ResponseMessagesUsers.PasswordResetSubject, htmlBody, null, null);
        if (!sent)
        {
            user.Activo = true;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
            throw new InvalidOperationException(ResponseMessagesEmailsController.SendEmailError);
        }

        return true;
    }

    public async Task<bool> ConfirmPasswordResetTicketAsync(string token)
    {
        var ticket = await _ticketRepository.GetTicketByTokenAsync(token);

        if (ticket == null || ticket.Estado != "Pending" || ticket.Expiracion < DateTime.Now)
            return false;

        ticket.Estado = "Accepted";
        await _ticketRepository.UpdateTicketAsync(ticket);
        await _ticketRepository.SaveChangesAsync();

        var pushNotification = new PushNotificationDTO
        {
            UserId = "admin",
            Title = "Ticket Aceptado",
            Message = $"El usuario {ticket.Usuario.Email} aceptó el ticket de reseteo.",
            Type = "SecurityAlert",
            AlertId = 0
        };

        await _hubContext.Clients.Group("admin").SendAsync("ReceiveAlertUpdate", pushNotification);

        return true;
    }

    public async Task<ResetStatusResponseDTO?> GetResetStatusAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null) return null;

        var ticket = await _ticketRepository.GetLatestTicketByUserIdAsync(user.Id);

        double? secondsRemaining = ticket != null && ticket.Estado == "Pending" && ticket.Expiracion > DateTime.Now
            ? (ticket.Expiracion - DateTime.Now).TotalSeconds
            : 0;

        return new ResetStatusResponseDTO
        {
            Email = user.Email,
            Status = ticket?.Estado ?? "None",
            Expiracion = ticket?.Expiracion,
            ExpiresInSeconds = secondsRemaining
        };
    }

    public async Task<List<ResetStatusResponseDTO>> GetPendingPasswordResetsAsync()
    {
        var tickets = await _ticketRepository.GetPendingAndAcceptedTicketsAsync();

        return tickets.Select(t =>
        {
            double? secondsRemaining = t.Estado == "Pending" && t.Expiracion > DateTime.Now
                ? (t.Expiracion - DateTime.Now).TotalSeconds
                : 0;

            return new ResetStatusResponseDTO
            {
                Email = t.Usuario.Email,
                Status = t.Estado,
                Expiracion = t.Expiracion,
                ExpiresInSeconds = secondsRemaining
            };
        }).ToList();
    }

    public async Task<bool> AssignNewPasswordAsync(string email, string newPassword)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null) return false;

        var ticket = await _ticketRepository.GetAcceptedTicketByUserIdAsync(user.Id);

        if (ticket == null) return false;

        user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.Activo = true;
        ticket.Estado = "Completed";

        _userRepository.Update(user);
        await _ticketRepository.UpdateTicketAsync(ticket);

        await _userRepository.SaveChangesAsync();
        await _ticketRepository.SaveChangesAsync();
        return true;
    }

    public async Task<string?> GetTemplatePasswordResetPage(string message)
    {
        var template = await _templateRepository.GetTemplateByTypeAsync(_templatesHTML.ConfirmChangePassPage ?? string.Empty);
        if (template == null) throw new InvalidOperationException("Template HTML no encontrado.");
        return template.HtmlCode.Replace("{message}", message);
    }
    public async Task<ChangeNameUsersDTO?> ChangeNameUsersAsync(int id, string newName, string newLastName, string newEmail, string newPhone)
    {
        try
        {
            var userName = await _userRepository.ChangeNameUsersAsync(id, newName, newLastName, newEmail, newPhone);

            if (userName == null) return null;

            return new ChangeNameUsersDTO
            {
                IdUser = userName.IdUser,
                NewUserName = userName.NewUserName,
                NewLastName = userName.NewLastName,
                NewEmail = userName.NewEmail,
                NewPhone = userName.NewPhone,
            };
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ex.Message);
        }
    }
    public async Task<Usuario?> SaveAvatarUrl(int userId, string avatarUrl)
    {
        try
        {
            var user = await _userRepository.SaveAvatarUrl(userId, avatarUrl);
            if (user == null) throw new InvalidOperationException(ResponseMessagesUsers.UserNotFound);
            return user;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ex.Message);
        }
    }
    public async Task<RolesRequestDTO> GetRolesAsync()
    {
        try
        {
            var roles = await _userRepository.GetRolesAsync();
            return roles;
        }
        catch (Exception)
        {
            throw new InvalidOperationException(ResponseMessagesUsers.RolesError);
        }
    }
}
