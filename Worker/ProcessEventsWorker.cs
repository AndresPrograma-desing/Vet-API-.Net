using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using vet_api_Net.Interfaze.Repositories;

//Describe: Worker de limpieza periódica de eventos de procesamiento de workers (worker_process_events) ya resueltos y vencidos según la retención configurada. No reintenta procesos, solo purga el histórico resuelto, igual que ClearNotificationsWorker.
namespace vet_api_Net.Worker;

public class ProcessEventsWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ProcessEventsWorker> _logger;
    private readonly bool _isEnabled;
    private readonly TimeSpan _interval;
    private readonly TimeSpan _retention;

    public ProcessEventsWorker(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<ProcessEventsWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        var section = configuration.GetSection("WorkerSettings:ProcessEventsWorker");
        _isEnabled = section.GetValue<bool>("Enabled");

        var intervalValue = section.GetValue<int>("IntervalValues");
        var intervalUnits = section.GetValue<string>("IntervalUnits")?.ToLower() ?? "hours";
        _interval = ToTimeSpan(intervalValue > 0 ? intervalValue : 24, intervalUnits, TimeSpan.FromHours(24));

        var retentionValue = section.GetValue<int>("RetentionValues");
        var retentionUnits = section.GetValue<string>("RetentionUnits")?.ToLower() ?? "days";
        _retention = ToTimeSpan(retentionValue > 0 ? retentionValue : 30, retentionUnits, TimeSpan.FromDays(30));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_isEnabled) return;

        _logger.LogInformation("Worker ProcessEventsWorker initialized.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("ProcessEventsWorker started.");

                using var scope = _serviceProvider.CreateScope();
                var workerProcessEventRepository = scope.ServiceProvider.GetRequiredService<IWorkerProcessEventRepository>();

                var threshold = DateTime.Now.Subtract(_retention);
                var expiredEvents = await workerProcessEventRepository.GetResolvedOlderThanAsync(threshold);

                if (expiredEvents.Count > 0)
                {
                    workerProcessEventRepository.RemoveRange(expiredEvents);
                    await workerProcessEventRepository.SaveChangesAsync();
                }

                _logger.LogInformation("ProcessEventsWorker finished.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico en el ciclo de ProcessEventsWorker.");
                _logger.LogInformation("ProcessEventsWorker finished.");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private static TimeSpan ToTimeSpan(int value, string units, TimeSpan fallback)
    {
        return units switch
        {
            "seconds" => TimeSpan.FromSeconds(value),
            "minutes" => TimeSpan.FromMinutes(value),
            "hours" => TimeSpan.FromHours(value),
            "days" => TimeSpan.FromDays(value),
            _ => fallback
        };
    }
}
