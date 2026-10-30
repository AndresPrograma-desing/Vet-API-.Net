using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DTOs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Utilities;
using vet_api_Net.Models;
using vet_api_Net.WorkerSettings;

namespace vet_api_Net.Worker
{
    public class DeleteConsultaPdfWorker : BackgroundService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<DeleteConsultaPdfWorker> _logger;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _config;
        private static readonly TimeSpan DisabledPollInterval = TimeSpan.FromMinutes(1);

        public DeleteConsultaPdfWorker(IWebHostEnvironment env, ILogger<DeleteConsultaPdfWorker> logger, IServiceProvider services, IConfiguration config)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        private static TimeSpan ParseInterval(int value, string? unit, TimeSpan fallback)
        {
            if (value <= 0 || string.IsNullOrWhiteSpace(unit)) return fallback;

            switch (unit.Trim().ToLowerInvariant())
            {
                case "s":
                case "sec":
                case "second":
                case "seconds":
                    return TimeSpan.FromSeconds(value);
                case "m":
                case "min":
                case "minute":
                case "minutes":
                    return TimeSpan.FromMinutes(value);
                case "h":
                case "hour":
                case "hours":
                    return TimeSpan.FromHours(value);
                case "d":
                case "day":
                case "days":
                    return TimeSpan.FromDays(value);
                default:
                    return TimeSpan.FromMinutes(value);
            }
        }

        private (bool Enabled, TimeSpan ScanInterval, TimeSpan Threshold) ResolveSettingsFromAppsettings()
        {
            var section = _config.GetSection("WorkerSettings:DeleteConsultaPdfWorker");
            var setting = new WorkerSetting();
            section.Bind(setting);

            var scanInterval = ParseInterval(setting.IntervalValues, setting.IntervalUnits, TimeSpan.FromMinutes(30));
            var threshold = ParseInterval(setting.RetentionValues, setting.RetentionUnits, TimeSpan.FromDays(30));

            return (setting.Enabled, scanInterval, threshold);
        }

        private (bool Enabled, TimeSpan ScanInterval, TimeSpan Threshold) ResolveSettingsFromDb(WorkerConfig? dbSetting, (bool Enabled, TimeSpan ScanInterval, TimeSpan Threshold) appsettingsFallback)
        {
            if (dbSetting == null) return appsettingsFallback;

            var scanInterval = ParseInterval(dbSetting.IntervalValue, dbSetting.IntervalUnit, appsettingsFallback.ScanInterval);
            var threshold = ParseInterval(dbSetting.RetentionValue ?? 0, dbSetting.RetentionUnit, appsettingsFallback.Threshold);

            return (dbSetting.IsEnabled, scanInterval, threshold);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                bool enabled;
                TimeSpan scanInterval;
                TimeSpan threshold;

                try
                {
                    var appsettingsValues = ResolveSettingsFromAppsettings();

                    if (_env.IsDevelopment())
                    {
                        (enabled, scanInterval, threshold) = appsettingsValues;
                    }
                    else
                    {
                        using var settingsScope = _services.CreateScope();
                        var workerConfigRepository = settingsScope.ServiceProvider.GetRequiredService<IWorkerConfigRepository>();
                        var dbSetting = await workerConfigRepository.GetByWorkerNameAsync(WorkerNames.DeleteConsultaPdfWorker, stoppingToken);
                        (enabled, scanInterval, threshold) = ResolveSettingsFromDb(dbSetting, appsettingsValues);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error al cargar la configuración del DeleteConsultaPdfWorker, se usarán valores predeterminados");
                    enabled = true;
                    scanInterval = TimeSpan.FromMinutes(30);
                    threshold = TimeSpan.FromDays(30);
                }

                if (!enabled)
                {
                    try
                    {
                        await Task.Delay(DisabledPollInterval, stoppingToken);
                    }
                    catch (TaskCanceledException) { }
                    continue;
                }

                try
                {
                    _logger.LogInformation("DeleteConsultaPdfWorker started.");

                    var webRoot = _env?.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    var dir = Path.Combine(webRoot, "consultas");

                    if (Directory.Exists(dir))
                    {
                        var now = DateTime.UtcNow;
                        var files = Directory.GetFiles(dir, "*.pdf");

                        using var scope = _services.CreateScope();
                        var consultasRepository = scope.ServiceProvider.GetRequiredService<IConsultasRepository>();

                        foreach (var file in files)
                        {
                            try
                            {
                                var created = File.GetCreationTimeUtc(file);
                                var age = now - created;
                                var fileName = Path.GetFileName(file);

                                if (age >= threshold)
                                {
                                    File.Delete(file);

                                    try
                                    {
                                        var escapedName = Uri.EscapeDataString(fileName);
                                        var consultasToUpdate = await consultasRepository.GetByDocumentFileNameAsync(fileName, escapedName);

                                        if (consultasToUpdate != null && consultasToUpdate.Count > 0)
                                        {
                                            foreach (var consulta in consultasToUpdate)
                                            {
                                                consulta.UrlDocx = null;
                                                consultasRepository.UpdateConsulta(consulta);
                                            }
                                            await consultasRepository.SaveChangesAsync();
                                        }
                                    }
                                    catch (Exception exUpd)
                                    {
                                        _logger.LogWarning(exUpd, "Error al limpiar Consulta.UrlDocx para el archivo {File}", fileName);
                                    }
                                }
                            }
                            catch (Exception exFile)
                            {
                                _logger.LogWarning(exFile, "Error al procesar el archivo {File}", file);
                            }
                        }
                    }

                    _logger.LogInformation("DeleteConsultaPdfWorker finished.");
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al escanear y eliminar PDFs de consultas expirados.");

                    using (var errorScope = _services.CreateScope())
                    {
                        var processEventUtilities = errorScope.ServiceProvider.GetRequiredService<IProcessEventUtilities>();
                        await processEventUtilities.RegisterErrorAsync(new RegisterProcessEventDTO
                        {
                            NameEvent = WorkerNames.DeleteConsultaPdfWorker,
                            TypeEvent = ProcessEventTypes.Error,
                            Title = "Fallo al escanear y eliminar PDFs de consultas expirados",
                            ErrorMessage = ex.Message
                        });
                    }

                    _logger.LogInformation("DeleteConsultaPdfWorker finished.");
                }

                try
                {
                    await Task.Delay(scanInterval, stoppingToken);
                }
                catch (TaskCanceledException) { }
            }
        }
    }
}
