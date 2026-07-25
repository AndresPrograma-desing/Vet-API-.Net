using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Constants;

namespace vet_api_Net.Services;

//Describe: Servicio para la gestión y almacenamiento de las reglas de respuesta y bases de conocimientos de la IA.
public class IaConocimientoService : IIaConocimientoService
{
    private readonly IIaConocimientoRepository _repository;

    public IaConocimientoService(IIaConocimientoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IaConocimiento?> GetByCategoriaAsync(string categoria)
    {
        if (string.IsNullOrWhiteSpace(categoria))
        {
            throw new ArgumentException(ResponseMessagesIaConocimiento.CategoryNotEmpty);
        }
        return await _repository.GetByCategoriaAsync(categoria);
    }

    public async Task<IaConocimiento> SaveConfigAsync(string categoria, string reglasRespuesta, string baseConocimiento)
    {
        if (string.IsNullOrWhiteSpace(categoria))
        {
            throw new ArgumentException(ResponseMessagesIaConocimiento.CategoryRequired);
        }

        var config = new IaConocimiento
        {
            Categoria = categoria.ToLower().Trim(),
            ReglasRespuesta = reglasRespuesta ?? string.Empty,
            BaseConocimiento = baseConocimiento ?? string.Empty,
            Creado = DateTime.Now,
            Actualizado = DateTime.Now
        };
        await _repository.AddAsync(config);

        await _repository.SaveChangesAsync();
        return config;
    }

    public async Task<List<IaConocimiento>> GetAllConfigsAsync()
    {
        return await _repository.GetAllAsync();
    }
}
