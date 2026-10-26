using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using vet_api_Net.Constants;
using vet_api_Net.Infrastructure.Configuration;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Utilities;
using vet_api_Net.Models;

//Describe: Background service que detecta mascotas pendientes de un resumen clínico por IA (sin historia previa, o con datos modificados tras el último análisis) y, al acumular un lote mínimo, las envía en un único request a Gemini y guarda el resumen devuelto por cada paciente. Su habilitación e intervalo son configurables desde la tabla worker_configs (fila "HistoriaClinicaSummaryWorker"), con appsettings.json como valores por defecto si no hay fila en la BD.
namespace vet_api_Net.Workers;

public class HistoriaClinicaSummaryWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<HistoriaClinicaSummaryWorker> _logger;
    private readonly bool _defaultEnabled;
    private readonly TimeSpan _defaultInterval;

    public HistoriaClinicaSummaryWorker(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<HistoriaClinicaSummaryWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        var section = configuration.GetSection("WorkerSettings:HistoriaClinicaSummaryWorker");
        _defaultEnabled = section.GetValue<bool>("Enabled");

        var intervalValue = section.GetValue<int>("IntervalValues");
        var intervalUnits = section.GetValue<string>("IntervalUnits");
        _defaultInterval = ParseInterval(intervalValue, intervalUnits) ?? TimeSpan.FromHours(12);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var interval = _defaultInterval;

            try
            {
                var (isEnabled, resolvedInterval) = await ResolveScheduleAsync(stoppingToken);
                interval = resolvedInterval;

                if (isEnabled)
                    await ProcessPendingSummariesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar los resúmenes clínicos por IA.");
            }

            try
            {
                await Task.Delay(interval, stoppingToken);
            }
            catch (TaskCanceledException) { }
        }
    }

    private async Task<(bool IsEnabled, TimeSpan Interval)> ResolveScheduleAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var workerConfigRepository = scope.ServiceProvider.GetRequiredService<IWorkerConfigRepository>();
        var config = await workerConfigRepository.GetByWorkerNameAsync(WorkerNames.HistoriaClinicaSummaryWorker, stoppingToken);

        if (config == null)
            return (_defaultEnabled, _defaultInterval);

        var interval = ParseInterval(config.IntervalValue, config.IntervalUnit) ?? _defaultInterval;
        return (config.IsEnabled, interval);
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

    private async Task ProcessPendingSummariesAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("HistoriaClinicaSummaryWorker started.");

        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IHistoriaClinicaRepository>();
        var geminiService = scope.ServiceProvider.GetRequiredService<IGeminiService>();
        var geminiOptions = scope.ServiceProvider.GetRequiredService<IOptions<GeminiOptions>>().Value;
        var processEventUtilities = scope.ServiceProvider.GetRequiredService<IProcessEventUtilities>();

        var batchMinSize = geminiOptions.BatchMinSize > 0 ? geminiOptions.BatchMinSize : 8;
        var pendingMascotas = await repository.GetMascotasPendingSummaryAsync(limit: 200);

        if (pendingMascotas.Count < batchMinSize)
        {
            _logger.LogInformation("Resúmenes clínicos pendientes: {Count}/{BatchMinSize}. Se espera al próximo ciclo para acumular el lote mínimo.", pendingMascotas.Count, batchMinSize);
            _logger.LogInformation("HistoriaClinicaSummaryWorker finished.");
            return;
        }

        var pacientesRequest = new List<GeminiPatientSummaryRequestDTO>();

        foreach (var mascota in pendingMascotas)
        {
            var consultas = await repository.GetConsultasByMascotaIdAsync(mascota.Id);
            var vacunas = await repository.GetPetVaccinationsByMascotaIdAsync(mascota.Id);
            var historiaActual = await repository.GetByMascotaIdAsync(mascota.Id);

            pacientesRequest.Add(new GeminiPatientSummaryRequestDTO
            {
                MascotaId = mascota.Id,
                Nombre = mascota.Nombre,
                Especie = mascota.Especie?.Nombre ?? string.Empty,
                Raza = mascota.Raza,
                Sexo = mascota.Sexo,
                Esterilizado = mascota.Esterilizado,
                FechaNacimiento = mascota.FechaNacimiento,
                Peso = mascota.Peso,
                Alergias = mascota.Alergias,
                CondicionesMedicas = mascota.CondicionesMedicas,
                Consultas = consultas.Select(c => new GeminiConsultaResumenDTO
                {
                    FechaConsulta = c.FechaConsulta,
                    Sintomas = c.Sintomas,
                    Diagnostico = c.Diagnostico,
                    Tratamiento = c.Tratamiento,
                    PesoActual = c.PesoActual
                }).ToList(),
                Vacunas = vacunas.Select(v => new GeminiVacunaResumenDTO
                {
                    NombreVacuna = v.Vaccine != null ? v.Vaccine.Name : string.Empty,
                    FechaVacunacion = v.ApplicationDate,
                    ProximaDosis = v.NextDoseDate
                }).ToList(),
                ResumenAnterior = historiaActual?.ResumenIa
            });
        }

        Dictionary<int, ResumenClinicoIAResponseDTO> resultados;
        try
        {
            resultados = await geminiService.GenerarResumenesBatchAsync(pacientesRequest);
            await processEventUtilities.ResolveAsync(WorkerNames.HistoriaClinicaSummaryWorker, GeminiBatchVinculationKey(pendingMascotas.Select(m => m.Id)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "No se pudo generar el lote de resúmenes clínicos con Gemini. Se reintentará en el próximo ciclo.");

            await processEventUtilities.RegisterErrorAsync(new RegisterProcessEventDTO
            {
                NameEvent = WorkerNames.HistoriaClinicaSummaryWorker,
                TypeEvent = ProcessEventTypes.Error,
                Title = $"Fallo al generar resúmenes clínicos con Gemini (lote de {pendingMascotas.Count} mascotas)",
                ErrorMessage = ex.Message,
                VinculationData = GeminiBatchVinculationKey(pendingMascotas.Select(m => m.Id))
            });

            _logger.LogInformation("HistoriaClinicaSummaryWorker finished.");
            return;
        }

        foreach (var mascota in pendingMascotas)
        {
            try
            {
                if (!resultados.TryGetValue(mascota.Id, out var resumen))
                {
                    _logger.LogWarning("Gemini no devolvió resumen para la mascota {MascotaId} en este lote.", mascota.Id);
                    continue;
                }

                var historiaAnterior = await repository.GetByMascotaIdAsync(mascota.Id);

                var nuevaHistoria = new HistoriaClinica
                {
                    MascotaId = mascota.Id,
                    ResumenIa = resumen.ResumenIa,
                    AlertasRiesgoIa = resumen.AlertasRiesgoIa,
                    SugerenciasIa = resumen.SugerenciasIa,
                    UltimoAnalisisIa = DateTime.Now,
                    NotasVeterinario = historiaAnterior?.NotasVeterinario,
                    Creado = DateTime.Now,
                    Actualizado = DateTime.Now
                };

                repository.Add(nuevaHistoria);

                await processEventUtilities.RegisterSuccessAsync(new RegisterProcessEventDTO
                {
                    NameEvent = WorkerNames.HistoriaClinicaSummaryWorker,
                    TypeEvent = ProcessEventTypes.Success,
                    Title = $"Resumen clínico generado con IA para {mascota.Nombre} (Mascota {mascota.Id})",
                    VinculationData = $"Mascota:{mascota.Id}",
                    Snapshot = JsonSerializer.Serialize(new
                    {
                        mascota.Id,
                        mascota.Nombre,
                        resumen.ResumenIa,
                        resumen.AlertasRiesgoIa,
                        resumen.SugerenciasIa
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al guardar el resumen clínico de la mascota {MascotaId}.", mascota.Id);
            }
        }

        await repository.SaveChangesAsync();

        _logger.LogInformation("HistoriaClinicaSummaryWorker finished.");
    }

    private static string GeminiBatchVinculationKey(IEnumerable<int> mascotaIds) =>
        $"HistoriaClinicaBatch:{string.Join(",", mascotaIds.OrderBy(id => id))}";
}
