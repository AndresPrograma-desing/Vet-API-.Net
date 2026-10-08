using System;
using System.Globalization;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Constants;
using vet_api_Net.Models;

namespace vet_api_Net.Services;

public class PetsService : IPetsService
{
    private readonly IPetsRepository _repository;
    private readonly IEspecieService _especieService;

    public PetsService(IPetsRepository repository, IEspecieService especieService)
    {
        _repository = repository;
        _especieService = especieService;
    }

    public async Task<MascotaListResponseDTO> GetAllMascotasAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
    {
        var (mascotas, totalCount) = await _repository.GetPagedWithRelationsAsync(pageNumber, pageSize, searchTerm);

        var items = mascotas.Select(mascota => new MascotaResumenDTO
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
            ImageUrl = mascota.ImageUrl,
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

        return new MascotaListResponseDTO { Items = items, TotalCount = totalCount };
    }

    public async Task<MascotaResumenDTO?> GetMascotaByIdAsync(int id)
    {
        var mascota = await _repository.GetByIdWithRelationsAsync(id);

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
            ImageUrl = mascota.ImageUrl,
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
        var pet = await _repository.GetByIdWithRelationsAsync(id);

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

        _repository.Update(pet);
        await _repository.SaveChangesAsync();

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
        => await _repository.DeleteWithDependenciesAsync(id);
}
