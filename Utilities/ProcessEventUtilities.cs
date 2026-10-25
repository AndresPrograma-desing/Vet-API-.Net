using System;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Utilities;
using vet_api_Net.Models;

//Describe: Soporte transversal usado por los workers para registrar en una tabla separada de logs_sistema los eventos de error/éxito de sus propios procesos, con conteo de intentos y vinculación al registro que se estaba procesando (snapshot JSON).
namespace vet_api_Net.Utilities;

public class ProcessEventUtilities(IWorkerProcessEventRepository workerProcessEventRepository) : IProcessEventUtilities
{
    private readonly IWorkerProcessEventRepository _workerProcessEventRepository = workerProcessEventRepository;

    public async Task<ProcessEventsDTO> RegisterErrorAsync(RegisterProcessEventDTO dto)
    {
        var existing = await _workerProcessEventRepository.GetActiveByNameAndVinculationAsync(dto.NameEvent, dto.VinculationData);

        if (existing != null)
        {
            existing.Intents += 1;
            existing.TypeEvent = dto.TypeEvent;
            existing.Title = dto.Title;
            existing.ErrorMessage = dto.ErrorMessage;
            existing.Snapshot = dto.Snapshot;
            existing.UpdatedAt = DateTime.Now;

            await _workerProcessEventRepository.SaveChangesAsync();
            return MapToDTO(existing);
        }

        var processEvent = new WorkerProcessEvent
        {
            NameEvent = dto.NameEvent,
            TypeEvent = dto.TypeEvent,
            Title = dto.Title,
            ErrorMessage = dto.ErrorMessage,
            VinculationData = dto.VinculationData,
            Snapshot = dto.Snapshot,
            Intents = 1,
            IsResolved = false,
            Created = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        await _workerProcessEventRepository.AddAsync(processEvent);
        await _workerProcessEventRepository.SaveChangesAsync();

        return MapToDTO(processEvent);
    }

    public async Task<ProcessEventsDTO> RegisterSuccessAsync(RegisterProcessEventDTO dto)
    {
        var processEvent = new WorkerProcessEvent
        {
            NameEvent = dto.NameEvent,
            TypeEvent = ProcessEventTypes.Success,
            Title = dto.Title,
            ErrorMessage = dto.ErrorMessage,
            VinculationData = dto.VinculationData,
            Snapshot = dto.Snapshot,
            Intents = 1,
            IsResolved = true,
            Created = DateTime.Now,
            UpdatedAt = DateTime.Now,
            ResolvedAt = DateTime.Now
        };

        await _workerProcessEventRepository.AddAsync(processEvent);
        await _workerProcessEventRepository.SaveChangesAsync();

        return MapToDTO(processEvent);
    }

    public async Task ResolveAsync(string nameEvent, string? vinculationData)
    {
        var existing = await _workerProcessEventRepository.GetActiveByNameAndVinculationAsync(nameEvent, vinculationData);
        if (existing == null) return;

        existing.IsResolved = true;
        existing.ResolvedAt = DateTime.Now;
        existing.UpdatedAt = DateTime.Now;

        await _workerProcessEventRepository.SaveChangesAsync();
    }

    private static ProcessEventsDTO MapToDTO(WorkerProcessEvent processEvent) => new()
    {
        Id = processEvent.Id,
        NameEvent = processEvent.NameEvent,
        TypeEvent = processEvent.TypeEvent,
        Title = processEvent.Title,
        ErrorMessage = processEvent.ErrorMessage,
        VinculationData = processEvent.VinculationData,
        Snapshot = processEvent.Snapshot,
        Intents = processEvent.Intents,
        IsResolved = processEvent.IsResolved,
        Created = processEvent.Created,
        UpdatedAt = processEvent.UpdatedAt,
        ResolvedAt = processEvent.ResolvedAt
    };
}
