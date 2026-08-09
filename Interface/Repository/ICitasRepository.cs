using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Repositories;

public interface ICitasRepository
{
    Task<List<Cita>> GetActiveForExpirationCheckAsync(int? doctorId, int? secretariaId);
    Task<(List<Cita> Items, int TotalCount)> GetPagedWithRelationsAsync(int? doctorId, int? secretariaId, DateTime? date, bool allDates, string? status, int pageNumber, int pageSize);
    Task<Cita?> GetByIdWithRelationsAsync(int id);
    Task<Cita?> GetByIdAsync(int id);
    Task<List<Cita>> GetByStatusAsync(string estado);
    Task<List<Cita>> GetByDateAsync(DateTime fecha);
    Task<List<Cita>> GetByClienteIdAsync(int clienteId);
    Task AddAsync(Cita cita);
    void Update(Cita cita);
    void Delete(Cita cita);
    Task<bool> SaveChangesAsync();
    Task<List<NotificationCitaDTO>> GetUpcomingNotificationsAsync(DateTime fecha, TimeOnly horaDesde, string dateFormat, string timeFormat, int? doctorId = null, int? secretariaId = null);
    // Task<Cita?> UpdateCitaAsync(int id);
    // Task<Cita?> GetCitaStatusAsync();
}