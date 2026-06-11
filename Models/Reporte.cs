using System;

namespace vet_api_Net.Models
{
    public class Reporte
    {
        public int Id { get; set; }
        public string? Titulo { get; set; } // Nombre o tipo de reporte
        public DateTime FechaCreacion { get; set; }
        public string? Categoria { get; set; } // ClientesMascotas, CitasConsultas, FacturacionPagos, InventarioProductos, VacunasTratamientos
        public string? Filtro { get; set; } // JSON con filtros aplicados (fechas, cliente, etc)
        public string? Datos { get; set; } // JSON con los datos del reporte
        public string? GeneradoPor { get; set; } // Usuario que generó el reporte
    }
}