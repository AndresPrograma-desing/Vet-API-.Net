using DTOs;
using NCrontab;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Constants;
using vet_api_Net.Data;
using vet_api_Net.HttpServices;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Models;
using vet_api_Net.Services;

namespace vet_api_Net.Workers;

public class BcvWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BcvWorker> _logger;
    private readonly IConfiguration _configuration;

    public BcvWorker(IServiceScopeFactory scopeFactory, ILogger<BcvWorker> logger, IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Servicio de actualización de Tasa BCV iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var settings = _configuration.GetSection("BcvSettings");
            bool isEnabled = settings.GetValue<bool>("WorkerEnabled", true);
            int targetId = settings.GetValue<int>("TargetId", 1);

            int intervalValue = settings.GetValue<int>("IntervalValue", 8);
            string intervalUnit = settings.GetValue<string>("IntervalUnit", "Hours") ?? "Hours";

            if (!isEnabled)
            {
                _logger.LogWarning("El Worker de BCV está desactivado en appsettings.json.");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                continue;
            }

            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var scraper = scope.ServiceProvider.GetRequiredService<IBcvScraper>();
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var notificationsPushService = scope.ServiceProvider.GetRequiredService<INotificationsPushService>();

                    _logger.LogInformation("Consultando precio en el BCV...");

                    decimal? precioActual = await scraper.ObtenerPrecioBcvAsync();
                    
                    if (precioActual.HasValue && precioActual.Value > 0)
                    { 
                        var moneyEntry = await context.MoneyTypes
                            .FirstOrDefaultAsync(m => m.Id == targetId, stoppingToken);

                        if (moneyEntry != null)
                        {
                            moneyEntry.BcvDollar = precioActual.Value;
                            moneyEntry.DollarPersistence = "USD";
                            moneyEntry.Fecha = DateTime.Now;
                              var data = new PushNotificationDTO
                            {
                                Message = ResponseMessagesMoneyTypes.BcvRequestSuccess,
                                Type = ResponseMessageNotificactionPush.Worker,
                                Title = ResponseMessagesMoneyTypes.UpdateTasaBcv,
                                UserId = targetId.ToString(),
                                NameUser = ResponseMessageNotificactionPush.System
                            };
                            await notificationsPushService.SendNotificationAsync(data);
                            _logger.LogInformation("Registro ID {Id} actualizado: {Precio} Bs.", targetId, precioActual);
                        }
                        else
                        {
                            var nuevaTasa = new MoneyType
                            {
                                Id = targetId,
                                BcvDollar = precioActual.Value,
                                DollarPersistence = "USD",
                                Fecha = DateTime.Now
                            };
                            context.MoneyTypes.Add(nuevaTasa);
                          
                            _logger.LogInformation("Registro ID {Id} creado por primera vez: {Precio} Bs.", targetId, precioActual);
                        }

                        await context.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation("Precio actualizado correctamente: {Precio} Bs.", precioActual);
                    }
                    else
                    {
                        var data = new PushNotificationDTO
                        {
                            Message = ResponseMessagesMoneyTypes.APIBcvFailDatails,
                            Type = ResponseMessageNotificactionPush.Error,
                            Title = ResponseMessagesMoneyTypes.APIBcvFail,
                            UserId = targetId.ToString(),
                            NameUser = ResponseMessageNotificactionPush.System
                        };
                        await notificationsPushService.SendNotificationAsync(data);
                        _logger.LogWarning("El scraper devolvió un valor inválido o nulo. No se actualizará la base de datos. Verifique la conexión o el sitio del BCV.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico en el BcvWorker.");
            }

            string cronExpression = settings.GetValue<string>("CronExpression", "0 9,17 * * *");
            TimeSpan delay = CalcularProximoRetardo(cronExpression);
 
            _logger.LogInformation("Próxima actualización programada vía Cron ('{Cron}') en {DelayHours} horas y {DelayMinutes} minutos.", cronExpression, (int)delay.TotalHours, delay.Minutes);
            await Task.Delay(delay, stoppingToken);
        }
    }

    private TimeSpan CalcularProximoRetardo(string cronExpression)
    {
        try
        {
            var schedule = CrontabSchedule.Parse(cronExpression);
            DateTime ahora = DateTime.Now;
            DateTime proximaEjecucion = schedule.GetNextOccurrence(ahora);
            return proximaEjecucion - ahora;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar la expresión Cron '{Cron}'. Usando fallback de 8 horas.", cronExpression);
            return TimeSpan.FromHours(8);
        }
    }
}