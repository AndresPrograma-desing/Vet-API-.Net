using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using vet_api_Net.Data;
using vet_api_Net.Interfaces.Services;
using DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Linq;
using vet_api_Net.WorkerSettings;

namespace vet_api_Net.Worker
{
    public class DeleteFacturaWorker : BackgroundService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<DeleteFacturaWorker> _logger;
        private readonly IServiceProvider _services;
        private TimeSpan _threshold;
        private TimeSpan _scanInterval;
        private readonly bool _enabled;

        public DeleteFacturaWorker(IWebHostEnvironment env, ILogger<DeleteFacturaWorker> logger, IServiceProvider services, IConfiguration config)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _services = services ?? throw new ArgumentNullException(nameof(services));

            try
            {
                var section = config.GetSection("WorkerSettings:DeleteFacturaWorker");
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
                            _scanInterval = TimeSpan.FromMinutes(setting.IntervalValues); break;
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
                            _threshold = TimeSpan.FromMinutes(setting.RetentionValues); break;
                    }
                }
                else
                {
                    _threshold = TimeSpan.FromMinutes(30);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al cargar la configuraciÃ³n del DeleteFacturaWorker, se usarÃ¡n valores predeterminados");
                _scanInterval = TimeSpan.FromSeconds(10);
                _threshold = TimeSpan.FromMinutes(30);
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
                    var webRoot = _env?.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    var dir = Path.Combine(webRoot, "facturas");

                    if (Directory.Exists(dir))
                    {
                        var now = DateTime.UtcNow;
                        var allowedExts = new[] { ".pdf", ".docx" };
                        var files = Directory.GetFiles(dir)
                            .Where(f => allowedExts.Contains(Path.GetExtension(f).ToLowerInvariant()))
                            .ToArray();

                        using var scope = _services.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var messagingService = scope.ServiceProvider.GetService<IMessagingService>();
                        var dbSetting = await db.FacturaConfigs.FirstOrDefaultAsync(stoppingToken);

                        var effectiveThreshold = _threshold;
                        if (dbSetting != null && !dbSetting.IsEnabled)
                        {
                            var minutosRetencion = dbSetting.Days > 0 ? dbSetting.Days : 30;
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
                                    var bot = await db.Usuarios.FirstOrDefaultAsync(u => u.Nombre == "Bot" || u.Email == "bottest@example.com");
                                    int? botId = bot?.Id;

                                    var secretarias = await db.Usuarios
                                        .Where(u => u.Rol == "secretaria" && (u.Activo == null || u.Activo == true))
                                        .ToListAsync();

                                    var messageText = $"La factura {fileName} ha expirado y fue eliminada automÃ¡ticamente.";

                                    if (botId.HasValue && messagingService != null)
                                    {
                                        foreach (var sec in secretarias)
                                        {
                                            try
                                            {
                                                var dto = new CreateMensajeDTO { EmisorId = botId.Value, ReceptorId = sec.Id, Contenido = messageText };
                                                await messagingService.SendMessageAsync(dto);
                                            }
                                            catch (Exception ex)
                                            {
                                                _logger.LogWarning(ex, "Error al enviar mensaje a la secretaria {Id}", sec.Id);
                                            }
                                        }
                                    }

                                    File.Delete(file);

                                    try
                                    {
                                        var escapedName = Uri.EscapeDataString(fileName);
                                        var facturasToUpdate = await db.Facturas
                                            .Where(f => !string.IsNullOrEmpty(f.UrlDocx) && (EF.Functions.Like(f.UrlDocx, "%" + fileName + "%") || EF.Functions.Like(f.UrlDocx, "%" + escapedName + "%")))
                                            .ToListAsync();

                                        if (facturasToUpdate != null && facturasToUpdate.Count > 0)
                                        {
                                            foreach (var fac in facturasToUpdate)
                                            {
                                                fac.UrlDocx = "la factura se ha eliminado";
                                                db.Facturas.Update(fac);
                                            }
                                            await db.SaveChangesAsync();
                                        }
                                    }
                                    catch (Exception exUpd)
                                    {
                                        _logger.LogWarning(exUpd, "Error al actualizar Factura.UrlDocx para el archivo {File}", fileName);
                                    }
                                }
                            }
                            catch (Exception exFile)
                            {
                                _logger.LogWarning(exFile, "Error al procesar el archivo {File}", file);
                            }
                        }
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al escanear y eliminar facturas expiradas.");
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