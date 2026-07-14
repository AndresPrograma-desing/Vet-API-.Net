using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DTOs;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Data;

namespace vet_api_Net.Services;

public class PetsService : IPetsService
{
    private readonly AppDbContext _context;

    public PetsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MascotaResumenDTO>> GetAllMascotasAsync()
    {
        var mascotas = await _context.Mascotas
            .Include(m => m.Cliente)
            .ToListAsync();

        if (mascotas == null || !mascotas.Any()) return new List<MascotaResumenDTO>();

        return mascotas.Select(mascota => new MascotaResumenDTO
        {
            Id = mascota.Id,
            ClienteId = mascota.ClienteId,
            Nombre = mascota.Nombre,
            Especie = mascota.Especie,
            Raza = mascota.Raza,
            Sexo = mascota.Sexo,
            FechaNacimiento = mascota.FechaNacimiento.HasValue ? mascota.FechaNacimiento.Value.ToString("yyyy-MM-dd") : null,
            Peso = mascota.Peso.HasValue ? mascota.Peso.Value.ToString("0.00", CultureInfo.InvariantCulture) : null,
            IdenteficacionMascota = mascota.IdenteficacionMascota,
            Color = mascota.Color,
            Alergias = mascota.Alergias,
            CondicionesMedicas = mascota.CondicionesMedicas,
            Esterilizado = mascota.Esterilizado,
            Cliente = mascota.Cliente != null ? new DetailsCitaDTO
            {
                Id = mascota.Cliente.Id,
                Nombre = mascota.Cliente.Nombre,
                Apellido = mascota.Cliente.Apellido,
                Email = mascota.Cliente.Email,
                Telefono = mascota.Cliente.Telefono,
                Identificacion = mascota.Cliente.Identificacion
            } : null
        }).ToList();
    }

    public async Task<MascotaResumenDTO?> GetMascotaByIdAsync(int id)
    {
        var mascota = await _context.Mascotas
            .Include(m => m.Cliente)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (mascota == null) return null;

        return new MascotaResumenDTO
        {
            Id = mascota.Id,
            ClienteId = mascota.ClienteId,
            Nombre = mascota.Nombre,
            Especie = mascota.Especie,
            Raza = mascota.Raza,
            Sexo = mascota.Sexo,
            FechaNacimiento = mascota.FechaNacimiento.HasValue ? mascota.FechaNacimiento.Value.ToString("yyyy-MM-dd") : null,
            Peso = mascota.Peso.HasValue ? mascota.Peso.Value.ToString("0.00", CultureInfo.InvariantCulture) : null,
            IdenteficacionMascota = mascota.IdenteficacionMascota,
            Color = mascota.Color,
            Alergias = mascota.Alergias,
            CondicionesMedicas = mascota.CondicionesMedicas,
            Esterilizado = mascota.Esterilizado,
            Cliente = mascota.Cliente != null ? new DetailsCitaDTO
            {
                Id = mascota.Cliente.Id,
                Nombre = mascota.Cliente.Nombre,
                Apellido = mascota.Cliente.Apellido,
                Email = mascota.Cliente.Email,
                Telefono = mascota.Cliente.Telefono,
                Identificacion = mascota.Cliente.Identificacion
            } : null
        };
    }
}
