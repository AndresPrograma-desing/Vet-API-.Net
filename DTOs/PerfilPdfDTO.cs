using System;
using System.Collections.Generic;

namespace DTOs
{
    public class PerfilPdfDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public DateTime Creado { get; set; }
        public DateTime Actualizado { get; set; }
        public List<FacturaPerfilDTO> Facturas { get; set; } = new List<FacturaPerfilDTO>();
        public List<MascotaPerfilDTO> Mascota { get; set; } = new List<MascotaPerfilDTO>();
    }

    public class MascotaPerfilDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Especie { get; set; }
        public string Raza { get; set; }
        public int? Edad { get; set; }
        public decimal? Peso { get; set; }
    }

    public class FacturaPerfilDTO
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
    }
}