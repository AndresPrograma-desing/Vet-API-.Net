using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Models;
using vet_api_Net.Interfaces.Repositories;
using vet_api_Net.Interfaces.Services;
using Microsoft.Extensions.Options;
using vet_api_Net.Infrastructure.Configuration;

namespace vet_api_Net.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _repository;
    private readonly ApiSettingsOptions _appSettings;

    public ClientService(IClientRepository repository, IOptions<ApiSettingsOptions> appSettings)
    {
        _repository = repository;
        _appSettings = appSettings.Value;
    }

    public async Task<Cliente?> GetClientAsync(int id)
    {
        var client = await _repository.GetByIdSimpleAsync(id);
        if (client == null) return null;

        return new Cliente
        {
            Id = client.Id,
            Nombre = client.Nombre,
            Apellido = client.Apellido,
            Email = client.Email,
            Telefono = client.Telefono
        };
    }

    public async Task<List<ClientListWithMascotasDTO>> GetAllClientsWithDetailsAsync()
    {
        var clients = await _repository.GetAllWithDetailsAsync();

        return clients.Select(c => new ClientListWithMascotasDTO
        {
            Id = c.Id,
            Nombre = c.Nombre,
            Apellido = c.Apellido,
            Email = c.Email,
            Telefono = c.Telefono,
            Direccion = c.Direccion,
            Identificacion = c.Identificacion,
            Nota = c.Nota,
            Creado = c.Creado,
            Actualizado = c.Actualizado,
            Mascotas = c.Mascota.Select(m => new MascotaResumenDTO
            {
                Id = m.Id,
                Nombre = m.Nombre,
                Especie = m.Especie,
                Raza = m.Raza,
                Color = m.Color,
                Sexo = m.Sexo,
                FechaNacimiento = m.FechaNacimiento?.ToString(_appSettings.DateFormat),
                Peso = m.Peso?.ToString("0.00", CultureInfo.InvariantCulture),
                IdenteficacionMascota = m.IdenteficacionMascota,
                Alergias = m.Alergias,
                CondicionesMedicas = m.CondicionesMedicas,
                Esterilizado = m.Esterilizado,
                Creado = m.Creado,
                Actualizado = m.Actualizado,
                Consultas = m.Consulta.Select(con => new ConsultaResumenDTO
                {
                    Id = con.Id,
                    Fecha = con.FechaConsulta
                }).ToList(),
                Citas = m.Cita.Select(ci => new CitaResumenDTO
                {
                    Id = ci.Id,
                    Fecha = ci.FechaCita
                }).ToList()
            }).ToList()
        }).ToList();
    }
}