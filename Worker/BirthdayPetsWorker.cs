using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using vet_api_Net.Constants;
using vet_api_Net.Infrastructure.Configuration;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Utilities;
using vet_api_Net.Models;

//Describe: Background service que detecta mascotas cuyo cumpleaños es hoy y aún no fueron notificadas este año, envía un email de felicitación al cliente y avisa al staff interno. Su habilitación e intervalo son configurables desde la tabla worker_configs (fila "BirthdayPetsWorker"), con appsettings.json como valores por defecto si no hay fila en la BD.
namespace vet_api_Net.Workers;

public class BirthdayPetsWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BirthdayPetsWorker> _logger;
    private readonly bool _defaultEnabled;
    private readonly TimeSpan _defaultInterval;

    public BirthdayPetsWorker(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<BirthdayPetsWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        var section = configuration.GetSection("WorkerSettings:BirthdayPetsWorker");
        _defaultEnabled = section.GetValue<bool>("Enabled");

        var intervalValue = section.GetValue<int>("IntervalValues");
        var intervalUnits = section.GetValue<string>("IntervalUnits");
        _defaultInterval = ParseInterval(intervalValue, intervalUnits) ?? TimeSpan.FromDays(1);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var interval = _defaultInterval;

            try
            {
                var (isEnabled, resolvedInterval) = await ResolveScheduleAsync(stoppingToken);
                interval = resolvedInterval;

                if (isEnabled)
                    await ProcessBirthdaysAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar los recordatorios de cumpleaños.");
            }

            try
            {
                await Task.Delay(interval, stoppingToken);
            }
            catch (TaskCanceledException) { }
        }
    }

    private async Task<(bool IsEnabled, TimeSpan Interval)> ResolveScheduleAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var workerConfigRepository = scope.ServiceProvider.GetRequiredService<IWorkerConfigRepository>();
        var config = await workerConfigRepository.GetByWorkerNameAsync(WorkerNames.BirthdayPetsWorker);

        if (config == null)
            return (_defaultEnabled, _defaultInterval);

        var interval = ParseInterval(config.IntervalValue, config.IntervalUnit) ?? _defaultInterval;
        return (config.IsEnabled, interval);
    }

    private static TimeSpan? ParseInterval(int value, string? unit)
    {
        if (value <= 0 || string.IsNullOrWhiteSpace(unit)) return null;

        return unit.Trim().ToLowerInvariant() switch
        {
            "seconds" => TimeSpan.FromSeconds(value),
            "minutes" => TimeSpan.FromMinutes(value),
            "hours" => TimeSpan.FromHours(value),
            "days" => TimeSpan.FromDays(value),
            _ => null
        };
    }

    private async Task ProcessBirthdaysAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BirthdayPetsWorker started.");

        using var scope = _serviceProvider.CreateScope();
        var petsRepository = scope.ServiceProvider.GetRequiredService<IPetsRepository>();
        var emailSenderService = scope.ServiceProvider.GetRequiredService<IEmailSenderService>();
        var notificationsPushService = scope.ServiceProvider.GetRequiredService<INotificationsPushService>();
        var templateRepository = scope.ServiceProvider.GetRequiredService<IEmailTemplateRepository>();
        var processEventUtilities = scope.ServiceProvider.GetRequiredService<IProcessEventUtilities>();
        var apiSettings = scope.ServiceProvider.GetRequiredService<IOptions<ApiSettingsOptions>>().Value;
        var templatesHTML = scope.ServiceProvider.GetRequiredService<IOptions<TemplatesHTML>>().Value;
        var systemName = apiSettings.SystemName ?? PdfText.HappyPets;

        var today = DateTime.Now;
        var pets = await petsRepository.GetDueForBirthdayReminderAsync(today.Month, today.Day, today.Year);

        if (pets.Count == 0)
        {
            _logger.LogInformation("BirthdayPetsWorker finished.");
            return;
        }

        var emailTemplate = await templateRepository.GetTemplateByTypeAsync(templatesHTML.BirthdayPets ?? "BirthdayPets");

        foreach (var pet in pets)
        {
            try
            {
                var clientEmail = pet.Cliente?.Email;
                var clientName = pet.Cliente == null ? string.Empty : $"{pet.Cliente.Nombre} {pet.Cliente.Apellido}".Trim();

                if (!string.IsNullOrWhiteSpace(clientEmail) && emailTemplate != null)
                {
                    var htmlBody = emailTemplate.HtmlCode
                        .Replace("{{system_name}}", systemName)
                        .Replace("{{client_name}}", clientName);

                    await emailSenderService.SendEmailAsync(clientEmail, ResponseMessagesBirthday.ReminderEmailSubject(systemName), htmlBody);
                }

                var pushData = new PushNotificationDTO
                {
                    Message = ResponseMessagesBirthday.NotificationMessage(pet.Nombre, clientName),
                    Type = ResponseMessageNotificactionPush.Worker,
                    Title = ResponseMessagesBirthday.NotificationTitle(pet.Nombre),
                    UserId = ResponseMessageNotificactionPush.Admin,
                    NameUser = ResponseMessageNotificactionPush.Admin
                };
                await notificationsPushService.SendNotificationAsync(pushData);

                pet.LastBirthdayNotifiedYear = today.Year;
                petsRepository.Update(pet);

                await processEventUtilities.RegisterSuccessAsync(new RegisterProcessEventDTO
                {
                    NameEvent = WorkerNames.BirthdayPetsWorker,
                    TypeEvent = ProcessEventTypes.Success,
                    Title = $"Recordatorio de cumpleaños enviado para {pet.Nombre}",
                    VinculationData = VinculationKey(pet.Id),
                    Snapshot = BuildBirthdaySnapshot(pet, clientEmail, clientName)
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al enviar el recordatorio de cumpleaños de la mascota {Id}.", pet.Id);

                await processEventUtilities.RegisterErrorAsync(new RegisterProcessEventDTO
                {
                    NameEvent = WorkerNames.BirthdayPetsWorker,
                    TypeEvent = ProcessEventTypes.Error,
                    Title = $"Fallo al enviar recordatorio de cumpleaños (Mascota {pet.Id})",
                    ErrorMessage = ex.Message,
                    VinculationData = VinculationKey(pet.Id),
                    Snapshot = BuildBirthdaySnapshot(pet, pet.Cliente?.Email, pet.Cliente != null ? $"{pet.Cliente.Nombre} {pet.Cliente.Apellido}".Trim() : string.Empty)
                });
            }
        }

        await petsRepository.SaveChangesAsync();

        _logger.LogInformation("BirthdayPetsWorker finished.");
    }

    private static string VinculationKey(int mascotaId) => $"Mascota:{mascotaId}";

    private static string BuildBirthdaySnapshot(Mascota pet, string? clientEmail, string? clientName)
    {
        return JsonSerializer.Serialize(new
        {
            pet.Id,
            pet.Nombre,
            ClienteId = pet.Cliente?.Id,
            ClienteNombre = clientName,
            ClienteEmail = clientEmail
        });
    }
}
