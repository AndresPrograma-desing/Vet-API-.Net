using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;
using DTOs;

namespace vet_api_Net.Interfaze.Repositories;

public interface ICitasRepository
{
    Task<List<Cita>> GetAllWithRelationsAsync();
    Task<Cita?> GetByIdWithRelationsAsync(int id);
    Task<Cita?> GetByIdAsync(int id);
    Task<List<Cita>> GetByStatusAsync(string estado);
    Task<List<Cita>> GetByDateAsync(DateTime fecha);
    Task AddAsync(Cita cita);
    void Update(Cita cita);
    void Delete(Cita cita);
    Task<bool> SaveChangesAsync();
    Task<List<NotificationCitaDTO>> GetUpcomingNotificationsAsync(DateTime fecha, TimeOnly horaDesde, string dateFormat, string timeFormat);
}