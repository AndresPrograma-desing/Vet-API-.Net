using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTOs;
using Microsoft.Extensions.Logging;
using vet_api_Net.Interfaze.Services;

//Describe: Implementación simulada de IGeminiService, usada cuando Mocks:Gemini está activo (ApiSettingsOptions.MocksOptions.Gemini) para probar HistoriaClinicaSummaryWorker sin consumir cuota real de Gemini.
namespace vet_api_Net.Mocks;

public class GeminiMock : IGeminiService
{
    private readonly ILogger<GeminiMock> _logger;

    public GeminiMock(ILogger<GeminiMock> logger)
    {
        _logger = logger;
    }

    public Task<Dictionary<int, ResumenClinicoIAResponseDTO>> GenerarResumenesBatchAsync(List<GeminiPatientSummaryRequestDTO> pacientes)
    {
        var generatedAt = DateTime.Now;
        _logger.LogInformation("Mocks:Gemini activo. Generando resúmenes simulados para {Count} paciente(s) a las {GeneratedAt}.", pacientes.Count, generatedAt);

        var resultados = pacientes.ToDictionary(p => p.MascotaId, p => new ResumenClinicoIAResponseDTO
        {
            ResumenIa = $"[MOCK generado {generatedAt:yyyy-MM-dd HH:mm:ss}] {p.Nombre} ({p.Especie}) se encuentra estable, sin hallazgos clínicos relevantes en las últimas {p.Consultas.Count} consulta(s) registrada(s).",
            AlertasRiesgoIa = p.Consultas.Count == 0 ? "Sin historial de consultas registrado." : string.Empty,
            SugerenciasIa = "Mantener controles veterinarios periódicos y esquema de vacunación al día."
        });

        return Task.FromResult(resultados);
    }
}
