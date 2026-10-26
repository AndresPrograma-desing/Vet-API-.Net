using System;
using System.Linq;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Models;

namespace vet_api_Net.Services;

public class HistorialPrecioService : IHistorialPrecioService
{
    private readonly IHistorialPrecioRepository _repository;

    public HistorialPrecioService(IHistorialPrecioRepository repository)
    {
        _repository = repository;
    }

    public async Task<HistorialPrecioListResponseDTO> GetHistorialAsync(
        int pageNumber,
        int pageSize,
        int? productId = null)
    {
        var (history, totalCount) = await _repository.GetPagedAsync(pageNumber, pageSize, productId);

        return new HistorialPrecioListResponseDTO
        {
            Items = history.Select(h => new HistorialPrecioDTO
            {
                Id = h.Id,
                ProductoId = h.ProductoId,
                ProductoNombre = h.Producto?.Nombre ?? string.Empty,
                PrecioAnterior = h.PrecioAnterior,
                PrecioNuevo = h.PrecioNuevo,
                UsuarioId = h.UsuarioId,
                UsuarioNombre = h.Usuario != null ? $"{h.Usuario.Nombre} {h.Usuario.Apellido}".Trim() : null,
                Motivo = h.Motivo,
                FechaCambio = h.FechaCambio
            }).ToList(),
            TotalCount = totalCount
        };
    }

    public async Task RegisterPriceChangeAsync(int productId, decimal previousPrice, decimal newPrice, int userId, string? reason = null)
    {
        var history = new HistorialPrecio
        {
            ProductoId = productId,
            PrecioAnterior = previousPrice,
            PrecioNuevo = newPrice,
            UsuarioId = userId,
            Motivo = reason,
            FechaCambio = DateTime.Now
        };

        _repository.AddHistory(history);
        await _repository.SaveChangesAsync();
    }
}
