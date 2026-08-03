using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using vet_api_Net.Models;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Infrastructure.Configuration;
using vet_api_Net.Constants;

namespace vet_api_Net.Services;

public class PasswordRecoveryService : IPasswordRecoveryService
{
    private readonly IPasswordRecoveryRepository _repository;
    private readonly IEmailSenderService _emailSenderService;
    private readonly ApiSettingsOptions _apiSettings;
    private readonly TemplatesHTML _templatesHTML;
    private static readonly Random _random = new Random();

    public PasswordRecoveryService(
        IPasswordRecoveryRepository repository,
        IEmailSenderService emailSenderService,
        IOptions<ApiSettingsOptions> apiSettingsOptions,
        IOptions<TemplatesHTML> templatesHTML
        )
    {
        _repository = repository;
        _emailSenderService = emailSenderService;
        _apiSettings = apiSettingsOptions.Value;
        _templatesHTML = templatesHTML.Value;
    }

    public async Task<bool> RequestRecoveryCodeAsync(string identifier)
{
    if (string.IsNullOrWhiteSpace(identifier)) return false;

    var user = await _repository.GetUserByEmailOrUsernameAsync(identifier);
    if (user == null || string.IsNullOrWhiteSpace(user.Email)) return false;

    var code = _random.Next(100000, 999999).ToString();
    var expirationMinutes = _apiSettings.PasswordRecoveryExpirationMinutes > 0
        ? _apiSettings.PasswordRecoveryExpirationMinutes
        : 15; 

    user.PasswordRecoveryCode = code;
    user.CodeRecoveryExpireDate = DateTime.Now.AddMinutes(expirationMinutes);

    _repository.UpdateUser(user);
    await _repository.SaveChangesAsync();

    var template = await _repository.GetTemplateByTypeAsync(_templatesHTML.ResetPassword!);
    var htmlBody = template?.HtmlCode ?? ResponseMessagesPasswordRecovery.DefaultResetPasswordHtml;

    var systemName = _apiSettings.SystemName ?? ResponseMessagesPasswordRecovery.DefaultSystemName;
    var fullName = $"{user.Nombre} {user.Apellido}".Trim();
    htmlBody = htmlBody.Replace("{{system_name}}", systemName);
    htmlBody = htmlBody.Replace("{{usuario}}", fullName);
    htmlBody = htmlBody.Replace("{{token_codigo}}", code);

    var subject = ResponseMessagesPasswordRecovery.RecoveryCodeSubject(systemName);
    
    // Siempre se envía el correo a la dirección de email registrada en la cuenta
    return await _emailSenderService.SendEmailAsync(user.Email, subject, htmlBody);
}

    public async Task<bool> VerifyCodeAndSendPasswordAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code)) return false;
        code = code.Trim();

        var user = await _repository.GetUserByRecoveryCodeAsync(code);
        if (user == null) return false;

        if (user.CodeRecoveryExpireDate == null || user.CodeRecoveryExpireDate.Value < DateTime.Now)
        {
            return false;
        }

        var template = await _repository.GetTemplateByTypeAsync(_templatesHTML.ResendPassword!);
        var htmlBody = template?.HtmlCode ?? ResponseMessagesPasswordRecovery.DefaultResendPasswordHtml;

        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var newPlainTextPassword = new string(Enumerable.Repeat(chars, 10)
            .Select(s => s[_random.Next(s.Length)]).ToArray());

        var systemName = _apiSettings.SystemName ?? ResponseMessagesPasswordRecovery.DefaultSystemName;
        var fullName = $"{user.Nombre} {user.Apellido}".Trim();
        htmlBody = htmlBody.Replace("{{system_name}}", systemName);
        htmlBody = htmlBody.Replace("{{client_name}}", fullName);
        htmlBody = htmlBody.Replace("{{nueva_clave}}", newPlainTextPassword);

        var subject = ResponseMessagesPasswordRecovery.ResendPasswordSubject(systemName);

        var sent = await _emailSenderService.SendEmailAsync(user.Email, subject, htmlBody);
        if (!sent) return false;

        user.PasswordRecoveryCode = ResponseMessagesPasswordRecovery.RequirePasswordChangeCode;
        user.CodeRecoveryExpireDate = null;
        user.Password = BCrypt.Net.BCrypt.HashPassword(newPlainTextPassword);

        _repository.UpdateUser(user);
        await _repository.SaveChangesAsync();

        return true;
    }


}
