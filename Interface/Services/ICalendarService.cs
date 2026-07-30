using System.Threading.Tasks;
using DTOs;
using static DTOs.CalendarDTO;

//Describe: Interfaz para el servicio de calendario, que define el contrato para recuperar la información del calendario formateado.
namespace vet_api_Net.Interfaze.Services;

public interface ICalendarService
{
    Task<CalendarResponseDTO> GetCalendarAsync(int year, int month, int? doctorId);
    Task<ValidateSlotResponseDTO> CheckDoctorAvailabilityAsync(int doctorId, DateTime date, TimeOnly requestedTime, int durationMinutes = 30);
}
