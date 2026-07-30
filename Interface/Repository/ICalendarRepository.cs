using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;

//Describe: Interfaz para el repositorio de calendario, que define los métodos de acceso a datos de citas por rango y doctores.

namespace vet_api_Net.Interfaze.Repositories;

public interface ICalendarRepository
{
    Task<List<Cita>> GetCitasForDoctorAsync(DateTime startDate, DateTime endDate, int? doctorId);
    Task<Usuario?> GetDoctorByIdAsync(int doctorId);
}
