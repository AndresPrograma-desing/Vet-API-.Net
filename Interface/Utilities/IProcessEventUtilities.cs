using System.Threading.Tasks;
using DTOs;

namespace vet_api_Net.Interfaze.Utilities;

public interface IProcessEventUtilities
{
    Task<ProcessEventsDTO> RegisterErrorAsync(RegisterProcessEventDTO dto);
    Task<ProcessEventsDTO> RegisterSuccessAsync(RegisterProcessEventDTO dto);
    Task ResolveAsync(string nameEvent, string? vinculationData);
}
