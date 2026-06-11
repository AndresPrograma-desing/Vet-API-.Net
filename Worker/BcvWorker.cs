using Microsoft.EntityFrameworkCore;
using vet_api_Net.HttpServices;
using vet_api_Net.Models;  
using vet_api_Net.Data;  

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

                    _logger.LogInformation("Consultando precio en el BCV...");
 
                    decimal precioActual = await scraper.ObtenerPrecioBcvAsync();

                    if (precioActual > 0)
                    { 
                        var moneyEntry = await context.MoneyTypes
                            .FirstOrDefaultAsync(m => m.Id == targetId, stoppingToken);

                        if (moneyEntry != null)
                        {  
                            moneyEntry.BcvDollar = precioActual;
                            moneyEntry.DollarPersistence = "USD";
                            moneyEntry.Fecha = DateTime.Now;
                            
                            _logger.LogInformation("Registro ID {Id} actualizado: {Precio} Bs.", targetId, precioActual);
                        }
                        else
                        {  
                            var nuevaTasa = new MoneyType
                            {
                                Id = targetId,
                                BcvDollar = precioActual,
                                DollarPersistence = "USD",
                                Fecha = DateTime.Now
                            };
                            context.MoneyTypes.Add(nuevaTasa);
                            _logger.LogInformation("Registro ID {Id} creado por primera vez: {Precio} Bs.", targetId, precioActual);
                        }
 
                        await context.SaveChangesAsync(stoppingToken);
                    }
                    else
                    {
                        _logger.LogWarning("El scraper devolvió 0. Verifique la conexión o el sitio del BCV.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico en el BcvWorker.");
            }

            TimeSpan delay = intervalUnit.ToLower() switch
            {
                "seconds" => TimeSpan.FromSeconds(intervalValue),
                "minutes" => TimeSpan.FromMinutes(intervalValue),
                "hours"   => TimeSpan.FromHours(intervalValue),
                "days"    => TimeSpan.FromDays(intervalValue),
                _         => TimeSpan.FromHours(8)
            };

            _logger.LogInformation("Próxima actualización en {Value} {Unit}.", intervalValue, intervalUnit);
            await Task.Delay(delay, stoppingToken);
        }
    }
}