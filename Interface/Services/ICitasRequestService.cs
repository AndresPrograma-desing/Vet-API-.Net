using DTOs;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Services;

public interface ICitasRequestService
{
    Task<List<CitasRequestDTO>> GetAllCitasRequestsAsync();
    Task<CitaDetalleDTO?> GetCitaRequestDetailsAsync(int id);
    Task<DeleteCitaDTO?> DeleteCitaAsync(int id);
    Task<StatusCitaRequestDTO> StatusCitaRequestAsync(int id);
    Task<StatusCitaRequestDTO?> UpdateCitaStatusAsync(int id, StatusCitaRequestDTO request);
    Task<CreateCitaDTO> CreateCitaAsync(CreateCitaDTO dto);
    Task<List<CitasRequestDTO>> CurrentCitaAsync();
    Task<List<NotificationCitaDTO>> NotificationCitaAsync();
}

// Describe => NotificationCitaAsync()

//Una cita debe de estar mayor a la hora actual en curso, pero no mayor a horas del dia 

//Ejemplo de entrada hora actual es 15:39
// Hora limite = 15:39 + 1 hora = 16:39
// Cita 1 = 15:00 (RECHAZADA)
// Cita 2 = 16:00 (ACEPTADA)
// Cita 3 = 17:00 (RECHAZADA)

// Cada cita debe de ser del mismo dia, si hay dos citas postuladas para ese dia solo notificara a la mas cercana a la hora en curso

// Describe => NotificationCitaAsync()

//An appointment must be greater than the current time in progress, but not greater than hours of the day

//Example of input current time is 15:39
// Deadline = 15:39 + 1 hour = 16:39
// Appointment 1 = 15:00 (REJECTED)
// Appointment 2 = 16:00 (ACCEPTED)
// Appointment 3 = 17:00 (REJECTED)

// Each appointment must be on the same day, if there are two appointments scheduled for that day it will only notify the one closest to the current time