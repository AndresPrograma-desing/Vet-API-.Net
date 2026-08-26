using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Models;

//Describe: Servicio de negocio para el catálogo compartido de especies (mascotas y vacunas), con semántica find-or-create.
namespace vet_api_Net.Services;

public class EspecieService : IEspecieService
{
    private readonly IEspecieRepository _repository;

    public EspecieService(IEspecieRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<EspecieDTO>> GetAllAsync()
    {
        var especies = await _repository.GetAllAsync();
        return especies.Select(MapToDto).ToList();
    }

    public async Task<EspecieDTO> CreateAsync(EspecieCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (string.IsNullOrWhiteSpace(dto.Nombre))
            throw new ArgumentException(ResponseMessagesEspecie.EspecieNombreRequired);

        var nombre = dto.Nombre.Trim();

        if (nombre.Length > 50)
            throw new ArgumentException(ResponseMessagesEspecie.EspecieMaxLength);

        var existente = await _repository.GetByNombreAsync(nombre);
        if (existente != null)
            throw new ArgumentException(ResponseMessagesEspecie.EspecieAlreadyExists);

        var especie = new Especie { Nombre = nombre };
        _repository.AddEspecie(especie);
        await _repository.SaveChangesAsync();

        return MapToDto(especie);
    }

    public async Task<Especie> GetOrCreateByNombreAsync(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException(ResponseMessagesEspecie.EspecieNombreRequired);

        var trimmed = nombre.Trim();

        var existente = await _repository.GetByNombreAsync(trimmed);
        if (existente != null)
            return existente;

        var especie = new Especie { Nombre = trimmed };
        _repository.AddEspecie(especie);
        await _repository.SaveChangesAsync();

        return especie;
    }

    private static EspecieDTO MapToDto(Especie especie) => new()
    {
        Id = especie.Id,
        Nombre = especie.Nombre
    };
}
