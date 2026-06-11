using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Models;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;

namespace vet_api_Net.Services;

public class ClientPetService : IClientPetService
{
    private readonly IClientPetRepository _repository;

    public ClientPetService(IClientPetRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateClientWithPetResponseDTO> CreateClientWithPetAsync(CreateClientWithPetDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var emailExists = await _repository.ExistsByEmailAsync(dto.Email);
        if (emailExists)
        {
            throw new InvalidOperationException(ResponseMessagesClient.ExistingEmail);
        }

        using var transaction = await _repository.BeginTransactionAsync();

        if(string.IsNullOrWhiteSpace(dto.Identificacion))
        {
            throw new ArgumentNullException(nameof(dto.Identificacion), ResponseMessagesClient.RequireIdentificacion);
        }

        var client = new Cliente
        {
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Email = dto.Email,
            Telefono = dto.Telefono,
            Direccion = dto.Direccion,
            Identificacion = dto.Identificacion,
            Nota = dto.Nota
        };

        _repository.AddClient(client);
        await _repository.SaveChangesAsync();

        var petDto = dto.Mascota;
        DateOnly? birthDate = null;
        if (!string.IsNullOrWhiteSpace(petDto.FechaNacimiento) && DateOnly.TryParse(petDto.FechaNacimiento, out var parsedDate))
        {
            birthDate = parsedDate;
        }

        var pet = new Mascota
        {
            ClienteId = client.Id,
            Nombre = petDto.Nombre,
            Especie = petDto.Especie,
            Raza = petDto.Raza,
            Color = petDto.Color,
            Sexo = petDto.Sexo,
            FechaNacimiento = birthDate,
            Peso = petDto.Peso,
            IdenteficacionMascota = petDto.IdenteficacionMascota,
            Alergias = petDto.Alergias,
            CondicionesMedicas = petDto.CondicionesMedicas,
            Esterilizado = petDto.Esterilizado
        };

        _repository.AddPet(pet);
        await _repository.SaveChangesAsync();

        await transaction.CommitAsync();

        return new CreateClientWithPetResponseDTO
        {
            ClienteId = client.Id,
            MascotaId = pet.Id,
            ClienteNombre = client.Nombre
        };
    }

public async Task<CreatePetForExistingClientResponseDTO> CreatePetForExistingClientAsync(CreatePetForExistingClientDTO dto)
{
    ArgumentNullException.ThrowIfNull(dto);

    var clientExists = await _repository.ClientExistsAsync(dto.ClienteId);
    if (!clientExists)
    {
        throw new KeyNotFoundException(ResponseMessagesClient.ClientNotFound);
    }

    if (!string.IsNullOrWhiteSpace(dto.IdenteficacionMascota))
    {
        var petExists = await _repository.PetExistsByIdentificacionAsync(dto.IdenteficacionMascota);
        if (petExists)
        {
            throw new InvalidOperationException(ResponseMessagesClient.RequireIdentificacionMascota);
        }
    }

    DateOnly? birthDate = null;
    if (!string.IsNullOrWhiteSpace(dto.FechaNacimiento) && DateOnly.TryParse(dto.FechaNacimiento, out var parsedDate))
    {
        birthDate = parsedDate;
    }

    var pet = new Mascota
    {
        ClienteId = dto.ClienteId,
        Nombre = dto.Nombre,
        Especie = dto.Especie,
        Raza = dto.Raza,
        Color = dto.Color,
        Sexo = dto.Sexo!,
        FechaNacimiento = birthDate,
        Peso = dto.Peso,
        IdenteficacionMascota = dto.IdenteficacionMascota?.Trim(),
        Alergias = dto.Alergias,
        CondicionesMedicas = dto.CondicionesMedicas,
        Esterilizado = dto.Esterilizado
    };

    _repository.AddPet(pet);
    await _repository.SaveChangesAsync();

    return new CreatePetForExistingClientResponseDTO
    {
        MascotaId = pet.Id,
        ClienteId = pet.ClienteId,
        MascotaNombre = pet.Nombre
    };
}
    public async Task<IEnumerable<ClientLookupResponseDTO>> GetClientsWithPetsLookupAsync(string searchTerm)
    {
        var clients = await _repository.GetClientsWithPetsLookupAsync(searchTerm);

        return clients.Select(c => new ClientLookupResponseDTO
        {
            Id = c.Id,
            Nombre = c.Nombre,
            Apellido = c.Apellido,
            Email = c.Email,
            Identificacion = c.Identificacion!,
            Mascotas = c.Mascota.Select(m => new PetLookupDTO
            {
                Id = m.Id,
                Nombre = m.Nombre
            }).ToList()
        });
    }
}