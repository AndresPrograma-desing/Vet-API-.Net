using System;
using System.Linq;
using System.Threading.Tasks;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Utilities;
using vet_api_Net.Models;

//Describe: Selecciona la siguiente credencial de Gemini disponible rotando entre todas las activas y respetando su cuota diaria (reset perezoso por fecha).
namespace vet_api_Net.Utilities;

public class GeminiApiKeySelector : IGeminiApiKeySelector
{
    private readonly IIntegrationCredentialRepository _repository;

    public GeminiApiKeySelector(IIntegrationCredentialRepository repository)
    {
        _repository = repository;
    }

    public async Task<IntegrationCredential?> AcquireAvailableKeyAsync()
    {
        var credentials = await _repository.GetActiveByProviderAsync(AiProviders.Gemini);
        var today = DateOnly.FromDateTime(DateTime.Now);
        var hasChanges = false;

        foreach (var credential in credentials)
        {
            if (credential.UsageDate != today)
            {
                credential.UsageDate = today;
                credential.UsageCount = 0;
                hasChanges = true;
            }
        }

        if (hasChanges)
        {
            foreach (var credential in credentials) _repository.Update(credential);
            await _repository.SaveChangesAsync();
        }

        return credentials.FirstOrDefault(c => c.UsageCount < c.DailyLimit);
    }

    public async Task RegisterUsageAsync(int credentialId)
    {
        var credential = await _repository.GetByIdAsync(credentialId);
        if (credential == null) return;

        credential.UsageCount += 1;
        credential.Actualizado = DateTime.Now;

        _repository.Update(credential);
        await _repository.SaveChangesAsync();
    }

    public async Task MarkExhaustedAsync(int credentialId)
    {
        var credential = await _repository.GetByIdAsync(credentialId);
        if (credential == null) return;

        credential.UsageCount = credential.DailyLimit;
        credential.Actualizado = DateTime.Now;

        _repository.Update(credential);
        await _repository.SaveChangesAsync();
    }
}
