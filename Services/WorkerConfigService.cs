using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Models;

namespace vet_api_Net.Services;

public class WorkerConfigService : IWorkerConfigService
{
    private readonly IWorkerConfigRepository _repository;

    public WorkerConfigService(IWorkerConfigRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<WorkerConfigDTO>> GetAllAsync()
    {
        var configs = await _repository.GetAllAsync();
        return configs.Select(MapToDTO).ToList();
    }

    public async Task<WorkerConfigDTO?> GetByWorkerNameAsync(string workerName)
    {
        var config = await _repository.GetByWorkerNameAsync(workerName);
        return config == null ? null : MapToDTO(config);
    }

    public async Task<WorkerConfigDTO> UpdateAsync(string workerName, UpdateWorkerConfigDTO data)
    {
        if (data.IntervalValue.HasValue && data.IntervalValue.Value <= 0)
            throw new ArgumentException(ResponseMessagesWorkerConfig.InvalidIntervalValue);

        var config = await _repository.GetByWorkerNameAsync(workerName);
        var isNew = config == null;
        config ??= new WorkerConfig { WorkerName = workerName };

        if (data.IsEnabled.HasValue) config.IsEnabled = data.IsEnabled.Value;
        if (data.IntervalValue.HasValue) config.IntervalValue = data.IntervalValue.Value;
        if (data.IntervalUnit != null) config.IntervalUnit = data.IntervalUnit;
        if (data.RetentionValue.HasValue) config.RetentionValue = data.RetentionValue.Value;
        if (data.RetentionUnit != null) config.RetentionUnit = data.RetentionUnit;
        if (data.GenerateEnabled.HasValue) config.GenerateEnabled = data.GenerateEnabled.Value;
        config.LastUpdated = DateTime.UtcNow;

        if (isNew)
            _repository.AddWorkerConfig(config);
        else
            _repository.UpdateWorkerConfig(config);

        await _repository.SaveChangesAsync();

        return MapToDTO(config);
    }

    private static WorkerConfigDTO MapToDTO(WorkerConfig config) => new()
    {
        WorkerName = config.WorkerName,
        IsEnabled = config.IsEnabled,
        IntervalValue = config.IntervalValue,
        IntervalUnit = config.IntervalUnit,
        RetentionValue = config.RetentionValue,
        RetentionUnit = config.RetentionUnit,
        GenerateEnabled = config.GenerateEnabled,
        LastUpdated = config.LastUpdated
    };
}
