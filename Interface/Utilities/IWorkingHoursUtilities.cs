//Describe: Contrato para validar que una fecha/hora esté dentro del horario laboral configurado en SystemConfig.
namespace vet_api_Net.Interfaze.Utilities;

public interface IWorkingHoursUtilities
{
    Task<(TimeOnly Start, TimeOnly End)> GetWorkingHoursAsync();
    Task EnsureWithinWorkingHoursAsync(TimeOnly time, int durationMinutes = 30);
}
