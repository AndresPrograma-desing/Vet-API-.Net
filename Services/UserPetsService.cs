using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Repositories;

namespace vet_api_Net.Services;

public class UserPetsService : IUserPetsService
{
    private readonly IPetsRepository _repository;

    public UserPetsService(IPetsRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<MascotaResumenDTO>> GetUserPetsAsync(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre)) return new List<MascotaResumenDTO>();

        var lower = nombre.Trim().ToLowerInvariant();

        var mascotas = await _repository.GetByClientNameWithRelationsAsync(lower);

        return mascotas.Select(m => new MascotaResumenDTO
        {
            Id = m.Id,
            ClienteId = m.ClienteId,
            Nombre = m.Nombre,
            Especie = m.Especie.Nombre,
            Raza = m.Raza,
            Sexo = m.Sexo,
            FechaNacimiento = m.FechaNacimiento.HasValue ? m.FechaNacimiento.Value.ToString("yyyy-MM-dd") : null,
            Peso = m.Peso.HasValue ? m.Peso.Value.ToString("0.00", CultureInfo.InvariantCulture) : null,
            IdenteficacionMascota = m.IdenteficacionMascota,
            Color = m.Color,
            Alergias = m.Alergias,
            CondicionesMedicas = m.CondicionesMedicas,
            Esterilizado = m.Esterilizado,
            ImageUrl = m.ImageUrl,
            Cliente = m.Cliente != null ? new DetailsCitaDTO
            {
                Id = m.Cliente.Id,
                Nombre = m.Cliente.Nombre,
                Apellido = m.Cliente.Apellido,
                Email = m.Cliente.Email,
                Telefono = m.Cliente.Telefono,
                Identificacion = m.Cliente.Identificacion
            } : null!
        }).ToList();
    }
}
