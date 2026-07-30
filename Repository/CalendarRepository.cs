using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Constants;
using vet_api_Net.Data;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Models;


//Describe: Repositorio para acceder a los datos de citas y doctores requeridos para el calendario.
namespace vet_api_Net.Repositories;

public class CalendarRepository : ICalendarRepository
{
    private readonly AppDbContext _context;

    public CalendarRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Cita>> GetCitasForDoctorAsync(DateTime startDate, DateTime endDate, int? doctorId)
    {
        var nextDayAfterEnd = endDate.AddDays(1);
        var query = _context.Citas
            .Include(c => c.Mascota)
                .ThenInclude(m => m.Cliente)
            .Include(c => c.Doctor)
            .Where(c => c.FechaCita >= startDate && c.FechaCita < nextDayAfterEnd);

        if (doctorId.HasValue)
        {
            query = query.Where(c => c.DoctorId == doctorId.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<Usuario?> GetDoctorByIdAsync(int doctorId)
    {
        return await _context.Usuarios
        .FirstOrDefaultAsync(u => u.Id == doctorId && EF.Functions.ILike(u.Rol, $"%{Roles.Doctor}%"));

    }
}
