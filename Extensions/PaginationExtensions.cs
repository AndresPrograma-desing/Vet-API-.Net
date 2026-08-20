using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Constants;

//Describe: Extensión reutilizable de paginación para IQueryable, usada por todos los repositorios que exponen listados paginados (Items + TotalCount). Centraliza normalización de pageNumber/pageSize y el Skip/Take/Count, para que agregar una regla nueva (ej. un tope distinto de pageSize) solo requiera tocar este archivo.
namespace vet_api_Net.Extensions;

public static class PaginationExtensions
{
    public static async Task<(List<T> Items, int TotalCount)> ToPagedResultAsync<T>(
        this IQueryable<T> query, int pageNumber, int pageSize)
    {
        var (normalizedPageNumber, normalizedPageSize) = NormalizePagination(pageNumber, pageSize);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((normalizedPageNumber - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public static async Task<List<T>> ToPagedListAsync<T>(
        this IQueryable<T> query, int pageNumber, int pageSize)
    {
        var (items, _) = await query.ToPagedResultAsync(pageNumber, pageSize);
        return items;
    }

    private static (int PageNumber, int PageSize) NormalizePagination(int pageNumber, int pageSize)
    {
        var normalizedPageNumber = pageNumber < 1 ? PaginationVariables.DefaultPageNumber : pageNumber;
        var normalizedPageSize = pageSize < 1
            ? PaginationVariables.DefaultPageSize
            : Math.Min(pageSize, PaginationVariables.MaxPageSize);

        return (normalizedPageNumber, normalizedPageSize);
    }
}
