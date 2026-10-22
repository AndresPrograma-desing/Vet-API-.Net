using DTOs;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;

//Describe: Background service que genera periódicamente el reporte completo del sistema (clientes, mascotas, productos, facturas, usuarios, citas, consultas, movimientos de inventario y un resumen de KPIs) y notifica al staff admin. La habilitación y el intervalo de ejecución son configurables desde la tabla worker_configs (fila "AutoGenerateReportWorker", editable desde el front vía WorkerConfigController), con appsettings.json como valores por defecto si no hay fila en la BD.
namespace vet_api_Net.Worker
{
    public class AutoGenerateReportWorker : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<AutoGenerateReportWorker> _logger;
        private readonly bool _defaultEnabled;
        private readonly TimeSpan _defaultInterval;

        public AutoGenerateReportWorker(IServiceProvider services, IConfiguration configuration, ILogger<AutoGenerateReportWorker> logger)
        {
            _services = services;
            _logger = logger;

            var section = configuration.GetSection("WorkerSettings:AutoGenerateReportWorker");
            _defaultEnabled = section.GetValue<bool>("Enabled");
            _defaultInterval = ParseInterval(section.GetValue<int>("IntervalValues"), section.GetValue<string>("IntervalUnits")) ?? TimeSpan.FromDays(15);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AutoGenerateReportWorker iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var interval = _defaultInterval;

                try
                {
                    var (generateEnabled, resolvedInterval) = await ResolveScheduleAsync(stoppingToken);
                    interval = resolvedInterval;

                    if (generateEnabled)
                        await GenerateReportAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en AutoGenerateReportWorker");
                    _logger.LogInformation("AutoGenerateReportWorker finished.");
                }

                try
                {
                    await Task.Delay(interval, stoppingToken);
                }
                catch (TaskCanceledException) { }
            }
        }

        private async Task<(bool GenerateEnabled, TimeSpan Interval)> ResolveScheduleAsync(CancellationToken stoppingToken)
        {
            using var scope = _services.CreateScope();
            var workerConfigRepository = scope.ServiceProvider.GetRequiredService<IWorkerConfigRepository>();
            var config = await workerConfigRepository.GetByWorkerNameAsync(WorkerNames.AutoGenerateReportWorker, stoppingToken);

            if (config == null)
                return (_defaultEnabled, _defaultInterval);

            var interval = ParseInterval(config.IntervalValue, config.IntervalUnit) ?? _defaultInterval;
            return (config.GenerateEnabled ?? _defaultEnabled, interval);
        }

        private async Task GenerateReportAsync()
        {
            _logger.LogInformation("AutoGenerateReportWorker started.");

            using var scope = _services.CreateScope();
            var reportService = scope.ServiceProvider.GetRequiredService<IReportSystemService>();
            var notificationsPushService = scope.ServiceProvider.GetRequiredService<INotificationsPushService>();

            _logger.LogInformation("Iniciando generación automática del reporte completo del sistema...");
            await reportService.GenerateFullSystemReportAsync(ResponseMessagesReport.GeneratedBySystem);

            var data = new PushNotificationDTO
            {
                Message = ResponseMessagesReport.ReportgeneratedDescription,
                Type = ResponseMessageNotificactionPush.Worker,
                Title = ResponseMessagesReport.ReportgeneratedTittle,
                UserId = ResponseMessageNotificactionPush.Admin,
                NameUser = ResponseMessageNotificactionPush.Admin
            };
            await notificationsPushService.SendNotificationAsync(data);
            _logger.LogInformation("Reporte automático generado con éxito.");
            _logger.LogInformation("AutoGenerateReportWorker finished.");
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
    }
}
