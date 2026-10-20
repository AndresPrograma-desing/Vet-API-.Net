using System.Threading.Tasks;
using vet_api_Net.Models;

//Describe: Contrato del selector de credenciales de Gemini, encargado de rotar entre múltiples API keys respetando la cuota diaria de cada una.
namespace vet_api_Net.Interfaze.Utilities;

public interface IGeminiApiKeySelector
{
    Task<IntegrationCredential?> AcquireAvailableKeyAsync();
    Task RegisterUsageAsync(int credentialId);
    Task MarkExhaustedAsync(int credentialId);
}
