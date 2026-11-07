using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Utilities;

//Describe: Valida que las citas (u otras operaciones con horario) caigan dentro del horario laboral guardado en SystemConfig.
namespace vet_api_Net.Utilities;

public class WorkingHoursUtilities(ISystemConfigRepository systemConfigRepository) : IWorkingHoursUtilities
{
    private static readonly TimeOnly DefaultStart = new(8, 0);
    private static readonly TimeOnly DefaultEnd = new(20, 0);

    public async Task<(TimeOnly Start, TimeOnly End)> GetWorkingHoursAsync()
    {
        var config = await systemConfigRepository.GetSystemConfigAsync();
        return config is null ? (DefaultStart, DefaultEnd) : (config.WorkStartTime, config.WorkEndTime);
    }

    public async Task EnsureWithinWorkingHoursAsync(TimeOnly time, int durationMinutes = 30)
    {
        var (start, end) = await GetWorkingHoursAsync();

        var endMinutes = time.Hour * 60 + time.Minute + durationMinutes;
        var endLimitMinutes = end.Hour * 60 + end.Minute;
        var startMinutes = start.Hour * 60 + start.Minute;
        var timeMinutes = time.Hour * 60 + time.Minute;

        if (timeMinutes < startMinutes || endMinutes > endLimitMinutes)
            throw new ArgumentException(ResponseMessagesCitas.OutsideWorkingHours(start, end));
    }
}
