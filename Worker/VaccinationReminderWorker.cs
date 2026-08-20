using System;
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

//Describe: Background service que detecta vacunas por vencer a 7 o 3 días, envía recordatorios por correo al cliente y notifica al staff interno para seguimiento telefónico. Su habilitación e intervalo son configurables desde la tabla worker_configs (fila "VaccinationReminderWorker"), con appsettings.json como valores por defecto si no hay fila en la BD.
namespace vet_api_Net.Workers;

public class VaccinationReminderWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<VaccinationReminderWorker> _logger;
    private readonly bool _defaultEnabled;
    private readonly TimeSpan _defaultInterval;

    public VaccinationReminderWorker(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<VaccinationReminderWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        var section = configuration.GetSection("WorkerSettings:VaccinationReminderWorker");
        _defaultEnabled = section.GetValue<bool>("Enabled");

        var intervalValue = section.GetValue<int>("IntervalValues");
        var intervalUnits = section.GetValue<string>("IntervalUnits");
        _defaultInterval = ParseInterval(intervalValue, intervalUnits) ?? TimeSpan.FromHours(1);
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
                    await ProcessRemindersAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar los recordatorios de vacunación.");
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
        var config = await workerConfigRepository.GetByWorkerNameAsync(WorkerNames.VaccinationReminderWorker);

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

    private async Task ProcessRemindersAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPetVaccinationRepository>();
        var emailSenderService = scope.ServiceProvider.GetRequiredService<IEmailSenderService>();
        var notificationsPushService = scope.ServiceProvider.GetRequiredService<INotificationsPushService>();
        var templateRepository = scope.ServiceProvider.GetRequiredService<IEmailTemplateRepository>();
        var apiSettings = scope.ServiceProvider.GetRequiredService<IOptions<ApiSettingsOptions>>().Value;
        var templatesHTML = scope.ServiceProvider.GetRequiredService<IOptions<TemplatesHTML>>().Value;
        var systemName = apiSettings.SystemName ?? PdfText.HappyPets;

        var today = DateOnly.FromDateTime(DateTime.Now);
        var sevenDayTarget = today.AddDays(VaccinationVariables.ReminderSevenDays);
        var threeDayTarget = today.AddDays(VaccinationVariables.ReminderThreeDays);

        var due = await repository.GetDueForReminderAsync(sevenDayTarget, threeDayTarget);
        if (due.Count == 0) return;

        var emailTemplate = await templateRepository.GetTemplateByTypeAsync(templatesHTML.VaccinePets ?? "VaccinePets");

        foreach (var petVaccination in due)
        {
            try
            {
                var petName = petVaccination.Mascota.Nombre;
                var vaccineName = petVaccination.Vaccine.Name;
                var dueDate = petVaccination.NextDoseDate!.Value.ToString("dd/MM/yyyy");
                var isSevenDayReminder = petVaccination.NextDoseDate == sevenDayTarget;

                var clientEmail = petVaccination.Mascota.Cliente?.Email;
                if (!string.IsNullOrWhiteSpace(clientEmail) && emailTemplate != null)
                {
                    var clientName = petVaccination.Mascota.Cliente == null
                        ? string.Empty
                        : $"{petVaccination.Mascota.Cliente.Nombre} {petVaccination.Mascota.Cliente.Apellido}".Trim();

                    var subject = ResponseMessagesVaccination.ReminderEmailSubject(systemName);
                    var htmlBody = emailTemplate.HtmlCode
                        .Replace("{{system_name}}", systemName)
                        .Replace("{{client_name}}", clientName)
                        .Replace("{{pet_name}}", petName)
                        .Replace("{{vaccine_name}}", vaccineName)
                        .Replace("{{due_date}}", dueDate);

                    await emailSenderService.SendEmailAsync(clientEmail, subject, htmlBody);
                }

                var pushData = new PushNotificationDTO
                {
                    Message = ResponseMessagesVaccination.PendingReminderNotificationMessage(petName, vaccineName, dueDate),
                    Type = ResponseMessageNotificactionPush.Worker,
                    Title = ResponseMessagesVaccination.PendingReminderNotificationTitle(petName),
                    UserId = ResponseMessageNotificactionPush.Admin,
                    NameUser = ResponseMessageNotificactionPush.Admin
                };
                await notificationsPushService.SendNotificationAsync(pushData);

                if (isSevenDayReminder)
                    petVaccination.ReminderSevenSentAt = DateTime.Now;
                else
                    petVaccination.ReminderThreeSentAt = DateTime.Now;

                repository.UpdatePetVaccination(petVaccination);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al enviar el recordatorio de vacunación {Id}.", petVaccination.Id);
            }
        }

        await repository.SaveChangesAsync();
    }
}
