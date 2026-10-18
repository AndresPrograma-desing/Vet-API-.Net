using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;

//Describe: Contrato del servicio que genera resúmenes clínicos por lote usando Gemini.
namespace vet_api_Net.Interfaze.Services;

public interface IGeminiService
{
    Task<Dictionary<int, ResumenClinicoIAResponseDTO>> GenerarResumenesBatchAsync(List<GeminiPatientSummaryRequestDTO> pacientes);
}
