using DTOs;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Services;
namespace vet_api_Net.Worker
{
    public class AutoGenerateReportWorker : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<AutoGenerateReportWorker> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(15);

        public AutoGenerateReportWorker(IServiceProvider services, ILogger<AutoGenerateReportWorker> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AutoGenerateReportWorker iniciado.");

            while (!stoppingToken.IsCancellationRequested)      
            {
                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var workerConfigRepository = scope.ServiceProvider.GetRequiredService<IWorkerConfigRepository>();
                        var reportRepository = scope.ServiceProvider.GetRequiredService<IReportRepository>();
                        var reportService = scope.ServiceProvider.GetRequiredService<IReportSystemService>();
                        var notificationsPushService = scope.ServiceProvider.GetRequiredService<INotificationsPushService>();

                        var config = await workerConfigRepository.GetByWorkerNameAsync(WorkerNames.AutoGenerateReportWorker, stoppingToken);
                        bool GenerateEnabled = config?.GenerateEnabled ?? false;

                        if (GenerateEnabled)
                        {
                            var ultimoReporteSistema = await reportRepository.GetLastByGeneratorAsync("sistema", stoppingToken);

                            // var proximaFechaGeneracion = ultimoReporteSistema?.FechaCreacion.AddDays(15) ?? DateTime.MinValue;
                            var proximaFechaGeneracion = ultimoReporteSistema?.FechaCreacion.AddSeconds(15) ?? DateTime.MinValue;
                            if (DateTime.UtcNow >= proximaFechaGeneracion)
                            {
                                _logger.LogInformation("Iniciando generación automática de reporte quincenal...");
                                await reportService.GenerateFullSystemReportAsync("sistema");
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
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en AutoGenerateReportWorker");
                }
                
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }
}