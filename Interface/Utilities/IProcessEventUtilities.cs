using DTOs;

namespace vet_api_Net.Interfaze.Utilities;

public interface IProcessEventUtilities
{
    Task<ProcessEventsDTO> ResgistreErrorAsync();
}