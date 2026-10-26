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
using vet_api_Net.WorkerSettings;

namespace vet_api_Net.Worker
{
    public class DeleteConsultaPdfWorker : BackgroundService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<DeleteConsultaPdfWorker> _logger;
        private readonly IServiceProvider _services;
        private TimeSpan _threshold;
        private TimeSpan _scanInterval;
        private readonly bool _enabled;

        public DeleteConsultaPdfWorker(IWebHostEnvironment env, ILogger<DeleteConsultaPdfWorker> logger, IServiceProvider services, IConfiguration config)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _services = services ?? throw new ArgumentNullException(nameof(services));

            try
            {
                var section = config.GetSection("WorkerSettings:DeleteConsultaPdfWorker");
                var setting = new WorkerSetting();
                section.Bind(setting);

                _enabled = setting.Enabled;

                if (setting.IntervalValues > 0 && !string.IsNullOrWhiteSpace(setting.IntervalUnits))
                {
                    var units = setting.IntervalUnits.Trim().ToLowerInvariant();
                    switch (units)
                    {
                        case "s":
                        case "sec":
                        case "second":
                        case "seconds":
                            _scanInterval = TimeSpan.FromSeconds(setting.IntervalValues); break;
                        case "m":
                        case "min":
                        case "minute":
                        case "minutes":
                            _scanInterval = TimeSpan.FromMinutes(setting.IntervalValues); break;
                        case "h":
                        case "hour":
                        case "hours":
                            _scanInterval = TimeSpan.FromHours(setting.IntervalValues); break;
                        case "d":
                        case "day":
                        case "days":
                            _scanInterval = TimeSpan.FromDays(setting.IntervalValues); break;
                        default:
                            _scanInterval = TimeSpan.FromMinutes(setting.IntervalValues); break;
                    }
                }
                else
                {
                    _scanInterval = TimeSpan.FromMinutes(30);
                }

                if (setting.RetentionValues > 0 && !string.IsNullOrWhiteSpace(setting.RetentionUnits))
                {
                    var units = setting.RetentionUnits.Trim().ToLowerInvariant();
                    switch (units)
                    {
                        case "s":
                        case "sec":
                        case "second":
                        case "seconds":
                            _threshold = TimeSpan.FromSeconds(setting.RetentionValues); break;
                        case "m":
                        case "min":
                        case "minute":
                        case "minutes":
                            _threshold = TimeSpan.FromMinutes(setting.RetentionValues); break;
                        case "h":
                        case "hour":
                        case "hours":
                            _threshold = TimeSpan.FromHours(setting.RetentionValues); break;
                        case "d":
                        case "day":
                        case "days":
                            _threshold = TimeSpan.FromDays(setting.RetentionValues); break;
                        default:
                            _threshold = TimeSpan.FromMinutes(setting.RetentionValues); break;
                    }
                }
                else
                {
                    _threshold = TimeSpan.FromDays(30);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al cargar la configuración del DeleteConsultaPdfWorker, se usarán valores predeterminados");
                _scanInterval = TimeSpan.FromMinutes(30);
                _threshold = TimeSpan.FromDays(30);
                _enabled = true;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_enabled)
            {
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
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
                        var workerConfigRepository = scope.ServiceProvider.GetRequiredService<IWorkerConfigRepository>();
                        var consultasRepository = scope.ServiceProvider.GetRequiredService<IConsultasRepository>();
                        var dbSetting = await workerConfigRepository.GetByWorkerNameAsync(WorkerNames.DeleteConsultaPdfWorker, stoppingToken);

                        var effectiveThreshold = _threshold;
                        if (dbSetting != null && !dbSetting.IsEnabled)
                        {
                            var minutosRetencion = dbSetting.RetentionValue > 0 ? dbSetting.RetentionValue!.Value : (int)_threshold.TotalMinutes;
                            effectiveThreshold = TimeSpan.FromMinutes(minutosRetencion);
                        }

                        foreach (var file in files)
                        {
                            try
                            {
                                var created = File.GetCreationTimeUtc(file);
                                var age = now - created;
                                var fileName = Path.GetFileName(file);

                                if (age >= effectiveThreshold)
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
                    await Task.Delay(_scanInterval, stoppingToken);
                }
                catch (TaskCanceledException) { }
            }
        }
    }
}
