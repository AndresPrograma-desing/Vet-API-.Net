using System;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using vet_api_Net.Models;
using vet_api_Net.Constants;
using vet_api_Net.Infrastructure.Configuration;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Services.Clients;
using vet_api_Net.Interfaze.Repositories.Clients;

namespace vet_api_Net.Services;

//Describe: Servicio de autenticación del Portal de Clientes vía código de un solo uso enviado por correo (sin contraseña: el cliente no crea ni recuerda ninguna clave).
public class ClientAuthService : IClientAuthService
{
    private readonly IClientRepository _clientRepository;
    private readonly IEmailSenderService _emailSenderService;
    private readonly IConfiguration _configuration;
    private readonly ApiSettingsOptions _apiSettings;
    private readonly TemplatesHTML _templatesHTML;
    private static readonly Random _random = new Random();

    public ClientAuthService(
        IClientRepository clientRepository,
        IEmailSenderService emailSenderService,
        IConfiguration configuration,
        IOptions<ApiSettingsOptions> apiSettingsOptions,
        IOptions<TemplatesHTML> templatesHTML)
    {
        _clientRepository = clientRepository;
        _emailSenderService = emailSenderService;
        _configuration = configuration;
        _apiSettings = apiSettingsOptions.Value;
        _templatesHTML = templatesHTML.Value;
    }

    public async Task<bool> RequestLoginCodeAsync(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier)) return false;

        var client = await _clientRepository.GetByIdentificacionOrEmailSimpleAsync(identifier);
        if (client == null || string.IsNullOrWhiteSpace(client.Email)) return false;

        var code = _random.Next(100000, 999999).ToString();
        var expirationMinutes = _apiSettings.PasswordRecoveryExpirationMinutes > 0
            ? _apiSettings.PasswordRecoveryExpirationMinutes
            : 15;

        client.PortalLoginCode = code;
        client.PortalLoginCodeExpireDate = DateTime.Now.AddMinutes(expirationMinutes);
        await _clientRepository.UpdateClientAsync(client);

        var template = await _clientRepository.GetTemplateByTypeAsync(_templatesHTML.ResetPassword!);
        var htmlBody = template?.HtmlCode ?? ResponseMessagesClientAuth.DefaultAccessCodeHtml;

        var systemName = _apiSettings.SystemName ?? ResponseMessagesClientAuth.DefaultSystemName;
        var fullName = $"{client.Nombre} {client.Apellido}".Trim();
        htmlBody = htmlBody.Replace("{{system_name}}", systemName);
        htmlBody = htmlBody.Replace("{{usuario}}", fullName);
        htmlBody = htmlBody.Replace("{{token_codigo}}", code);

        var subject = ResponseMessagesClientAuth.AccessCodeSubject(systemName);

        return await _emailSenderService.SendEmailAsync(client.Email, subject, htmlBody);
    }

    public async Task<Cliente?> VerifyLoginCodeAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code)) return null;
        code = code.Trim();

        var client = await _clientRepository.GetByPortalLoginCodeAsync(code);
        if (client == null) return null;

        if (client.PortalLoginCodeExpireDate == null || client.PortalLoginCodeExpireDate.Value < DateTime.Now)
            return null;

        client.PortalLoginCode = null;
        client.PortalLoginCodeExpireDate = null;
        await _clientRepository.UpdateClientAsync(client);

        return client;
    }

    public string GenerateToken(Cliente client)
    {
        ArgumentNullException.ThrowIfNull(client);

        var key = _configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(key)) throw new InvalidOperationException("JWT key is not configured.");

        var tokenHandler = new JwtSecurityTokenHandler();
        var keyBytes = Encoding.UTF8.GetBytes(key);

        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, client.Id.ToString()),
            new Claim(ClaimTypes.Email, client.Email ?? string.Empty),
            new Claim(ClaimTypes.Role, Roles.Cliente)
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
}
