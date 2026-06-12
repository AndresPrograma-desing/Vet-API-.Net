using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Interfaces.Services;
using vet_api_Net.Interfaces.Repositories;

namespace vet_api_Net.Services
{
    public class PetsService : IPetsService
    {
        private readonly IPetsRepository _petsRepository;

        public PetsService(IPetsRepository petsRepository)
        {
            _petsRepository = petsRepository;
        }

        public async Task<List<MascotaResumenDTO>> GetAllMascotasAsync()
        {
            var mascotas = await _petsRepository.GetAllMascotasWithClienteAsync();

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
            var mascota = await _petsRepository.GetMascotaByIdWithClienteAsync(id);

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
}
