using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Models;

//Describe: Administra las credenciales de integraciones externas (ej. API keys de Gemini) por proveedor, sin exponer el valor secreto en las respuestas.
namespace vet_api_Net.Services;

public class IntegrationCredentialService : IIntegrationCredentialService
{
    private readonly IIntegrationCredentialRepository _repository;

    public IntegrationCredentialService(IIntegrationCredentialRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<IntegrationCredentialDTO>> GetByProviderAsync(string provider)
    {
        var credentials = await _repository.GetActiveByProviderAsync(provider);
        return credentials.Select(MapToDto).ToList();
    }

    public async Task<IntegrationCredentialDTO> CreateAsync(CreateIntegrationCredentialDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Provider))
            throw new ArgumentException(ResponseMessagesIntegrationCredential.ProviderRequired);

        if (string.IsNullOrWhiteSpace(dto.SecretValue))
            throw new ArgumentException(ResponseMessagesIntegrationCredential.SecretValueRequired);

        if (dto.DailyLimit <= 0)
            throw new ArgumentException(ResponseMessagesIntegrationCredential.InvalidDailyLimit);

        var credential = new IntegrationCredential
        {
            Provider = dto.Provider.Trim().ToLowerInvariant(),
            SecretValue = dto.SecretValue.Trim(),
            Label = dto.Label,
            DailyLimit = dto.DailyLimit,
            UsageCount = 0,
            UsageDate = DateOnly.FromDateTime(DateTime.Now),
            IsActive = dto.IsActive,
            Creado = DateTime.Now,
            Actualizado = DateTime.Now
        };

        _repository.Add(credential);
        await _repository.SaveChangesAsync();

        return MapToDto(credential);
    }

    public async Task<IntegrationCredentialDTO> UpdateAsync(int id, UpdateIntegrationCredentialDTO dto)
    {
        var credential = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException(ResponseMessagesIntegrationCredential.NotFound);

        if (dto.SecretValue != null)
            credential.SecretValue = dto.SecretValue.Trim();

        if (dto.Label != null)
            credential.Label = dto.Label;

        if (dto.DailyLimit.HasValue)
        {
            if (dto.DailyLimit.Value <= 0)
                throw new ArgumentException(ResponseMessagesIntegrationCredential.InvalidDailyLimit);

            credential.DailyLimit = dto.DailyLimit.Value;
        }

        if (dto.IsActive.HasValue)
            credential.IsActive = dto.IsActive.Value;

        credential.Actualizado = DateTime.Now;

        _repository.Update(credential);
        await _repository.SaveChangesAsync();

        return MapToDto(credential);
    }

    public async Task DeleteAsync(int id)
    {
        var credential = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException(ResponseMessagesIntegrationCredential.NotFound);

        _repository.Delete(credential);
        await _repository.SaveChangesAsync();
    }

    private static IntegrationCredentialDTO MapToDto(IntegrationCredential credential) => new()
    {
        Id = credential.Id,
        Provider = credential.Provider,
        Label = credential.Label,
        DailyLimit = credential.DailyLimit,
        UsageCount = credential.UsageCount,
        UsageDate = credential.UsageDate,
        IsActive = credential.IsActive
    };
}
