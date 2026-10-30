using System;
using System.ComponentModel.DataAnnotations;

namespace vet_api_Net.Validation;

// Rechaza fechas futuras en campos que semánticamente solo tienen sentido en el pasado o presente
// (fecha de nacimiento, fecha de aplicación de una vacuna, etc.). Acepta DateOnly, DateOnly?,
// DateTime, DateTime? y string (parseado igual que los services del proyecto, vía DateOnly.TryParse),
// para poder aplicarse tanto a los DTOs que ya tipan la fecha como a los que aún la reciben como
// string libre (ej. CreateMascotaDTO.FechaNacimiento).
public class NotFutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null) return true; // [Required] se encarga de los obligatorios

        DateOnly? date = value switch
        {
            DateOnly d => d,
            DateTime dt => DateOnly.FromDateTime(dt),
            string s when DateOnly.TryParse(s, out var parsed) => parsed,
            string s when string.IsNullOrWhiteSpace(s) => null,
            string => null, // string no parseable: no es responsabilidad de este atributo
            _ => null
        };

        if (date == null) return true;

        return date.Value <= DateOnly.FromDateTime(DateTime.UtcNow);
    }
}
