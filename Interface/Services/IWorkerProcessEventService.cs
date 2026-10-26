using System;
using System.Threading.Tasks;
using DTOs;

namespace vet_api_Net.Interfaze.Services;

public interface IWorkerProcessEventService
{
    Task<ProcessEventsListResponseDTO> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? typeEvent,
        bool? isResolved,
        DateTime? fromDate,
        DateTime? toDate);
}
