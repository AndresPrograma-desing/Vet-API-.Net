using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using vet_api_Net.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Linq;
using vet_api_Net.WorkerSettings;

namespace vet_api_Net.Worker
{
    public class DeleteReportWorker : BackgroundService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<DeleteReportWorker> _logger;
        private readonly IServiceProvider _services;
        private TimeSpan _threshold;
        private TimeSpan _scanInterval;
        private readonly bool _enabled;

        public DeleteReportWorker(IWebHostEnvironment env, ILogger<DeleteReportWorker> logger, IServiceProvider services, IConfiguration config)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _services = services ?? throw new ArgumentNullException(nameof(services));

            try
            {
                var section = config.GetSection("WorkerSettings:DeleteReportWorker");
                var setting = new WorkerSetting();
                section.Bind(setting);

                _enabled = setting.Enabled;
 
                if (setting.IntervalValues > 0 && !string.IsNullOrWhiteSpace(setting.IntervalUnits))
                {
                    var units = setting.IntervalUnits.Trim().ToLowerInvariant();
                    switch (units)
                    {
                        case "s": case "sec": case "second": case "seconds":
                            _scanInterval = TimeSpan.FromSeconds(setting.IntervalValues); break;
                        case "m": case "min": case "minute": case "minutes":
                            _scanInterval = TimeSpan.FromMinutes(setting.IntervalValues); break;
                        case "h": case "hour": case "hours":
                            _scanInterval = TimeSpan.FromHours(setting.IntervalValues); break;
                        case "d": case "day": case "days":
                            _scanInterval = TimeSpan.FromDays(setting.IntervalValues); break;
                        default:
                            _scanInterval = TimeSpan.FromSeconds(20); break;
                    }
                }
                else
                {
                    _scanInterval = TimeSpan.FromSeconds(20);
                }
 
                if (setting.RetentionValues > 0 && !string.IsNullOrWhiteSpace(setting.RetentionUnits))
                {
                    var units = setting.RetentionUnits.Trim().ToLowerInvariant();
                    switch (units)
                    {
                        case "s": case "sec": case "second": case "seconds":
                            _threshold = TimeSpan.FromSeconds(setting.RetentionValues); break;
                        case "m": case "min": case "minute": case "minutes":
                            _threshold = TimeSpan.FromMinutes(setting.RetentionValues); break;
                        case "h": case "hour": case "hours":
                            _threshold = TimeSpan.FromHours(setting.RetentionValues); break;
                        case "d": case "day": case "days":
                            _threshold = TimeSpan.FromDays(setting.RetentionValues); break;
                        default:
                            _threshold = TimeSpan.FromSeconds(10); break;
                    }
                }
                else
                {
                    _threshold = TimeSpan.FromSeconds(10);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al cargar la configuración de DeleteReportWorker. Usando valores seguros por defecto.");
                _scanInterval = TimeSpan.FromSeconds(10);
                _threshold = TimeSpan.FromSeconds(30);
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
                    using (var scope = _services.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        
                        var dbSetting = await db.ReporConfigs.FirstOrDefaultAsync(stoppingToken);

                        DateTime deadline;

                        if (dbSetting != null && !dbSetting.IsEnabled)
                        {
                            int diasRetencion = dbSetting.Days > 0 ? dbSetting.Days : 30;
                            deadline = DateTime.UtcNow.AddMinutes(-diasRetencion);
                        }
                        else
                        { 
                            deadline = DateTime.UtcNow - _threshold;
                        }

                        var expireReports = await db.Reportes
                            .Where(r => r.FechaCreacion <= deadline)
                            .ToListAsync(stoppingToken);

                        if (expireReports.Any())
                        {
                            var webRoot = _env?.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                            var dir = Path.Combine(webRoot, "Reports");

                            foreach (var reporte in expireReports)
                            {
                                try
                                {
                                    if (Directory.Exists(dir))
                                    { 
                                        var archives = Directory.GetFiles(dir, $"Reporte_{reporte.Id}_*.xlsx");
                                        foreach (var arch in archives)
                                        {
                                            if (File.Exists(arch))
                                            {
                                                File.Delete(arch);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogWarning(ex, "No se pudo borrar el archivo físico del reporte {Id}", reporte.Id);
                                }
                            }
 
                            db.Reportes.RemoveRange(expireReports);
                            await db.SaveChangesAsync(stoppingToken);
                        }
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
 
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error crítico en el ciclo de DeleteReportWorker");
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