using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using vet_api_Net.Interfaces.Repositories;

namespace vet_api_Net.Workers;

public class ClearNotificationsWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly bool _isEnabled;
    private readonly ILogger<ClearNotificationsWorker> _logger;
    private readonly TimeSpan _interval;
    private int _totalDeleted;
    private DateTime _nextExecution;

    public ClearNotificationsWorker(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<ClearNotificationsWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        
        var section = configuration.GetSection("WorkerSettings:ClearNotificationsWorker");
        _isEnabled = section.GetValue<bool>("Enabled");
        
        var intervalValue = section.GetValue<int>("IntervalValues");
        var intervalUnits = section.GetValue<string>("IntervalUnits")?.ToLower() ?? "hours";

        _interval = intervalUnits switch
        {
            "seconds" => TimeSpan.FromSeconds(intervalValue),
            "minutes" => TimeSpan.FromMinutes(intervalValue),
            "hours" => TimeSpan.FromHours(intervalValue),
            "days" => TimeSpan.FromDays(intervalValue),
            _ => TimeSpan.FromHours(24)
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_isEnabled) return;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var alertsRepository = scope.ServiceProvider.GetRequiredService<IAlertsRepository>();
                
                _totalDeleted = await alertsRepository.ExecutePurgeReadAlertsAsync();
                _nextExecution = DateTime.Now.Add(_interval);

                _logger.LogInformation(
                    "Eliminacion de notificaciones completada a las {Time}. Intervalo: {Interval}. Total eliminadas: {TotalDeleted}. \n \t Proxima ejecucion: {NextExecution}.", 
                    DateTimeOffset.Now, 
                    _interval, 
                    _totalDeleted, 
                    _nextExecution.ToString("yyyy-MM-dd HH:mm:ss")
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while clearing notifications.");   
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}