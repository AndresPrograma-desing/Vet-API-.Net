using System;
using System.Collections.Generic;
using System.Linq;
using DTOs;

namespace vet_api_Net.Utilities;

public static class CalendarHelperUtilities
{
    private static readonly List<MonthInfoDTO> SpanishMonths =
[
    new() { Number = 1, Name = "Enero" },
    new() { Number = 2, Name = "Febrero" },
    new() { Number = 3, Name = "Marzo" },
    new() { Number = 4, Name = "Abril" },
    new() { Number = 5, Name = "Mayo" },
    new() { Number = 6, Name = "Junio" },
    new() { Number = 7, Name = "Julio" },
    new() { Number = 8, Name = "Agosto" },
    new() { Number = 9, Name = "Septiembre" },
    new() { Number = 10, Name = "Octubre" },
    new() { Number = 11, Name = "Noviembre" },
    new() { Number = 12, Name = "Diciembre" }
];


    /// <summary>
    /// Devuelve el listado estático de meses en español.
    /// </summary>
    public static List<MonthInfoDTO> GetMonths()
    {
        return SpanishMonths;
    }

    /// <summary>
    /// Genera un rango de años alrededor del año actual.
    /// </summary>
    /// <param name="yearsBefore">Años hacia atrás (default 3)</param>
    /// <param name="yearsAfter">Años hacia adelante (default 3)</param>
    public static List<int> GetAvailableYears(int yearsBefore = 3, int yearsAfter = 3)
    {
        int currentSystemYear = DateTime.Now.Year;
        int count = yearsBefore + 1 + yearsAfter;

        return Enumerable.Range(currentSystemYear - yearsBefore, count).ToList();
    }
}