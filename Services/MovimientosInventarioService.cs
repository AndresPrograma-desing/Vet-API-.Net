using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Models;

namespace vet_api_Net.Services;

public class MovimientosInventarioService : IMovimientosInventarioService
{
    private readonly IMovimientosInventarioRepository _repository;

    public MovimientosInventarioService(IMovimientosInventarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<MovimientoInventarioListResponseDTO> GetMovimientosAsync(
        int pageNumber,
        int pageSize,
        int? productId = null,
        string? movementType = null)
    {
        var (movements, totalCount) = await _repository.GetPagedAsync(pageNumber, pageSize, productId, movementType);

        return new MovimientoInventarioListResponseDTO
        {
            Items = movements.Select(m => new MovimientoInventarioDTO
            {
                Id = m.Id,
                ProductoId = m.ProductoId,
                ProductoNombre = m.Producto?.Nombre ?? string.Empty,
                TipoMovimiento = m.TipoMovimiento,
                Cantidad = m.Cantidad,
                StockAnterior = m.StockAnterior,
                StockNuevo = m.StockNuevo,
                Motivo = m.Motivo,
                ReferenciaId = m.ReferenciaId,
                UsuarioId = m.UsuarioId,
                UsuarioNombre = m.Usuario != null ? $"{m.Usuario.Nombre} {m.Usuario.Apellido}".Trim() : null,
                FechaMovimiento = m.FechaMovimiento
            }).ToList(),
            TotalCount = totalCount
        };
    }

    public async Task RegisterMovementAsync(int productId, string movementType, int quantity, int previousStock, int newStock, string reason, int userId, int? referenceId = null)
    {
        var movement = new MovimientosInventario
        {
            ProductoId = productId,
            TipoMovimiento = movementType,
            Cantidad = quantity,
            StockAnterior = previousStock,
            StockNuevo = newStock,
            Motivo = reason,
            ReferenciaId = referenceId,
            UsuarioId = userId,
            FechaMovimiento = DateTime.Now
        };

        _repository.AddMovement(movement);
        await _repository.SaveChangesAsync();
    }

    public async Task<List<ProductoStockBajoDTO>> GetLowStockAlertsAsync()
    {
        var products = await _repository.GetProductsWithLowStockAsync();

        return products.Select(p => new ProductoStockBajoDTO
        {
            Id = p.Id,
            Codigo = p.Codigo,
            Nombre = p.Nombre,
            Stock = p.Stock,
            StockMinimo = p.StockMinimo
        }).ToList();
    }
}
