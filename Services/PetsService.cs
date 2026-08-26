using System;
using System.Globalization;
using System.Threading.Tasks;
using DTOs;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Constants;
using vet_api_Net.Models;

namespace vet_api_Net.Services;

public class PetsService : IPetsService
{
    private readonly AppDbContext _context;
    private readonly IEspecieService _especieService;

    public PetsService(AppDbContext context, IEspecieService especieService)
    {
        _context = context;
        _especieService = especieService;
    }

    public async Task<List<MascotaResumenDTO>> GetAllMascotasAsync()
    {
        var mascotas = await _context.Mascotas
            .Include(m => m.Cliente)
            .Include(m => m.Especie)
            .ToListAsync();

        if (mascotas == null || !mascotas.Any()) return new List<MascotaResumenDTO>();

        return mascotas.Select(mascota => new MascotaResumenDTO
        {
            Id = mascota.Id,
            ClienteId = mascota.ClienteId,
            Nombre = mascota.Nombre,
            Especie = mascota.Especie.Nombre,
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
            .Include(m => m.Especie)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (mascota == null) return null;

        return new MascotaResumenDTO
        {
            Id = mascota.Id,
            ClienteId = mascota.ClienteId,
            Nombre = mascota.Nombre,
            Especie = mascota.Especie.Nombre,
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

    public async Task<MascotaResumenDTO?> UpdateMascotaAsync(int id, UpdatePetDTO dto)
    {
        var pet = await _context.Mascotas
            .Include(m => m.Cliente)
            .Include(m => m.Especie)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (pet == null) return null;

        var especie = await _especieService.GetOrCreateByNombreAsync(dto.Especie);

        pet.Nombre = dto.Nombre;
        pet.EspecieId = especie.Id;
        pet.Especie = especie;
        pet.Raza = dto.Raza;
        pet.Color = dto.Color;
        pet.Sexo = dto.Sexo;

        if (!string.IsNullOrWhiteSpace(dto.FechaNacimiento) && DateOnly.TryParse(dto.FechaNacimiento, out DateOnly fn))
        {
            if (fn.Year > DateTime.Now.Year || fn > DateOnly.FromDateTime(DateTime.Now))
            {
                throw new InvalidOperationException(ResponseMessagesClient.InvalidBirthDate);
            }
            pet.FechaNacimiento = fn;
        }

        pet.Peso = dto.Peso;
        pet.IdenteficacionMascota = dto.IdenteficacionMascota;
        pet.Alergias = dto.Alergias;
        pet.CondicionesMedicas = dto.CondicionesMedicas;
        pet.Esterilizado = dto.Esterilizado;
        pet.Actualizado = DateTime.Now;

        _context.Mascotas.Update(pet);
        await _context.SaveChangesAsync();

        return new MascotaResumenDTO
        {
            Id = pet.Id,
            ClienteId = pet.ClienteId,
            Nombre = pet.Nombre,
            Especie = pet.Especie.Nombre,
            Raza = pet.Raza,
            Sexo = pet.Sexo,
            FechaNacimiento = pet.FechaNacimiento.HasValue ? pet.FechaNacimiento.Value.ToString("yyyy-MM-dd") : null,
            Peso = pet.Peso.HasValue ? pet.Peso.Value.ToString("0.00", CultureInfo.InvariantCulture) : null,
            IdenteficacionMascota = pet.IdenteficacionMascota,
            Color = pet.Color,
            Alergias = pet.Alergias,
            CondicionesMedicas = pet.CondicionesMedicas,
            Esterilizado = pet.Esterilizado,
            Cliente = pet.Cliente != null ? new DetailsCitaDTO
            {
                Id = pet.Cliente.Id,
                Nombre = pet.Cliente.Nombre,
                Apellido = pet.Cliente.Apellido,
                Email = pet.Cliente.Email,
                Telefono = pet.Cliente.Telefono,
                Identificacion = pet.Cliente.Identificacion
            } : null
        };
    }

    public async Task<bool> DeleteMascotaAsync(int id)
    {
        // 1. Delete associated ConsultasProductos
        var consultaIds = await _context.Consultas
            .Where(c => c.MascotaId == id)
            .Select(c => c.Id)
            .ToListAsync();

        if (consultaIds.Any())
        {
            await _context.ConsultasProductos
                .Where(cp => consultaIds.Contains(cp.ConsultaId))
                .ExecuteDeleteAsync();
        }

        // 2. Delete associated DetallesFacturas
        var facturaIds = await _context.Facturas
            .Where(f => f.MascotaId == id)
            .Select(f => f.Id)
            .ToListAsync();

        if (facturaIds.Any())
        {
            await _context.DetallesFacturas
                .Where(df => facturaIds.Contains(df.FacturaId))
                .ExecuteDeleteAsync();
        }

        // 3. Delete Facturas
        await _context.Facturas
            .Where(f => f.MascotaId == id)
            .ExecuteDeleteAsync();

        // 4. Delete PetVaccinations
        await _context.PetVaccinations
            .Where(v => v.MascotaId == id)
            .ExecuteDeleteAsync();

        // 5. Delete HistoriasClinicas
        await _context.HistoriasClinicas
            .Where(h => h.MascotaId == id)
            .ExecuteDeleteAsync();

        // 6. Delete Citas
        await _context.Citas
            .Where(c => c.MascotaId == id)
            .ExecuteDeleteAsync();

        // 7. Delete Consultas
        await _context.Consultas
            .Where(c => c.MascotaId == id)
            .ExecuteDeleteAsync();

        // 8. Delete Mascota
        var affected = await _context.Mascotas
            .Where(m => m.Id == id)
            .ExecuteDeleteAsync();

        return affected > 0;
    }
}
