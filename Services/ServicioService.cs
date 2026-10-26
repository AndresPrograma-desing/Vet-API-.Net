using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Models;

namespace vet_api_Net.Services;

public class ServicioService : IServicioService
{
    private readonly IServicioRepository _repository;

    public ServicioService(IServicioRepository repository)
    {
        _repository = repository;
    }

    public async Task<ServicioListResponseDTO> GetServiciosAsync(int pageNumber = 1, int pageSize = 10, bool? active = null, string? searchTerm = null)
    {
        var (servicios, totalCount) = await _repository.GetPagedAsync(pageNumber, pageSize, active, searchTerm);

        return new ServicioListResponseDTO
        {
            Items = servicios.Select(MapToDTO).ToList(),
            TotalCount = totalCount
        };
    }

    public async Task<ServicioDTO?> GetByIdAsync(int id)
    {
        var servicio = await _repository.GetByIdAsync(id);
        return servicio == null ? null : MapToDTO(servicio);
    }

    public async Task<ServicioDTO> CreateAsync(ServicioCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (string.IsNullOrWhiteSpace(dto.Nombre))
            throw new ArgumentException(ResponseMessagesServicio.ServicioNombreRequired);

        if (dto.Precio < 0)
            throw new ArgumentException(ResponseMessagesServicio.ServicioPrecioInvalid);

        if (await _repository.ExistsByNameAsync(dto.Nombre))
            throw new ArgumentException(ResponseMessagesServicio.ServicioNombreAlreadyExists);

        var servicio = new Servicio
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            Precio = dto.Precio,
            Activo = dto.Activo,
            Creado = DateTime.Now,
            Actualizado = DateTime.Now
        };

        _repository.AddServicio(servicio);
        await _repository.SaveChangesAsync();

        return MapToDTO(servicio);
    }

    public async Task<ServicioDTO> UpdateAsync(int id, ServicioUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var servicio = await _repository.GetByIdAsync(id);
        if (servicio == null)
            throw new KeyNotFoundException(ResponseMessagesServicio.ServicioNotFound);

        if (dto.Precio.HasValue && dto.Precio < 0)
            throw new ArgumentException(ResponseMessagesServicio.ServicioPrecioInvalid);

        if (!string.IsNullOrWhiteSpace(dto.Nombre) && await _repository.ExistsByNameAsync(dto.Nombre, id))
            throw new ArgumentException(ResponseMessagesServicio.ServicioNombreAlreadyExists);

        servicio.Nombre = string.IsNullOrWhiteSpace(dto.Nombre) ? servicio.Nombre : dto.Nombre;
        servicio.Descripcion = dto.Descripcion ?? servicio.Descripcion;
        servicio.Precio = dto.Precio ?? servicio.Precio;
        servicio.Activo = dto.Activo ?? servicio.Activo;
        servicio.Actualizado = DateTime.Now;

        _repository.UpdateServicio(servicio);
        await _repository.SaveChangesAsync();

        return MapToDTO(servicio);
    }

    public async Task DeleteAsync(int id)
    {
        var servicio = await _repository.GetByIdAsync(id);
        if (servicio == null)
            throw new KeyNotFoundException(ResponseMessagesServicio.ServicioNotFound);

        _repository.DeleteServicio(servicio);
        await _repository.SaveChangesAsync();
    }

    private static ServicioDTO MapToDTO(Servicio servicio) => new()
    {
        Id = servicio.Id,
        Nombre = servicio.Nombre,
        Descripcion = servicio.Descripcion,
        Precio = servicio.Precio,
        Activo = servicio.Activo,
        Creado = servicio.Creado
    };
}
