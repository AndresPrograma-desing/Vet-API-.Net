using System;
using System.Linq;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;

//Describe: Orquesta la consulta paginada y filtrada de eventos de procesamiento de workers (errores/éxitos con reintentos), separada del log de auditoría HTTP general.
namespace vet_api_Net.Services;

public class WorkerProcessEventService(IWorkerProcessEventRepository workerProcessEventRepository) : IWorkerProcessEventService
{
    private readonly IWorkerProcessEventRepository _workerProcessEventRepository = workerProcessEventRepository;

    public async Task<ProcessEventsListResponseDTO> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? typeEvent,
        bool? isResolved,
        DateTime? fromDate,
        DateTime? toDate)
    {
        var (items, totalCount) = await _workerProcessEventRepository.GetPagedAsync(
            pageNumber, pageSize, typeEvent, isResolved, fromDate, toDate);

        var dtoItems = items.Select(e => new ProcessEventsDTO
        {
            Id = e.Id,
            NameEvent = e.NameEvent,
            TypeEvent = e.TypeEvent,
            Title = e.Title,
            ErrorMessage = e.ErrorMessage,
            VinculationData = e.VinculationData,
            Snapshot = e.Snapshot,
            Intents = e.Intents,
            IsResolved = e.IsResolved,
            Created = e.Created,
            UpdatedAt = e.UpdatedAt,
            ResolvedAt = e.ResolvedAt
        }).ToList();

        return new ProcessEventsListResponseDTO
        {
            Items = dtoItems,
            TotalCount = totalCount
        };
    }
}
