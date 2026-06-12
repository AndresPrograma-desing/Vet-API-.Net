using vet_api_Net.Data;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Models;
using DTOs;

namespace vet_api_Net.Interfaces.Services;

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