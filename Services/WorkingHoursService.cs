using System.Globalization;
using DTOs;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Utilities;

//Describe: Lógica para leer y actualizar el horario laboral global (hora de apertura y cierre) guardado en SystemConfig.
namespace vet_api_Net.Services;

public class WorkingHoursService(ISystemConfigRepository systemConfigRepository, IWorkingHoursUtilities workingHours) : IWorkingHoursService
{
    private const string TimeFormat = "HH:mm";

    public async Task<WorkingHoursDTO> GetAsync()
    {
        var (start, end) = await workingHours.GetWorkingHoursAsync();
        return ToDto(start, end);
    }

    public async Task<WorkingHoursDTO> UpdateAsync(WorkingHoursDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (!TimeOnly.TryParseExact(dto.StartTime, TimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var start) ||
            !TimeOnly.TryParseExact(dto.EndTime, TimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var end))
            throw new ArgumentException(ResponseMessagesWorkingHours.InvalidFormat);

        if (start >= end)
            throw new ArgumentException(ResponseMessagesWorkingHours.StartMustBeBeforeEnd);

        await systemConfigRepository.UpdateWorkingHoursAsync(start, end);
        return ToDto(start, end);
    }

    private static WorkingHoursDTO ToDto(TimeOnly start, TimeOnly end) => new()
    {
        StartTime = start.ToString(TimeFormat, CultureInfo.InvariantCulture),
        EndTime = end.ToString(TimeFormat, CultureInfo.InvariantCulture)
    };
}
