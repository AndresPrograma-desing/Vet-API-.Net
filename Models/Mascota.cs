using System;
using System.Collections.Generic;

namespace vet_api_Net.Models;

public partial class Mascota
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public string Nombre { get; set; } = null!;

    public int EspecieId { get; set; }

    public string? Raza { get; set; }

    public string? Color { get; set; }

    public string Sexo { get; set; } = null!;

    public DateOnly? FechaNacimiento { get; set; }

    public decimal? Peso { get; set; }

    public string? IdenteficacionMascota { get; set; }

    public string? Alergias { get; set; }

    public string? CondicionesMedicas { get; set; }

    public bool? Esterilizado { get; set; }

    public DateTime Creado { get; set; }

    public DateTime Actualizado { get; set; }

    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual Especie Especie { get; set; } = null!;

    public virtual ICollection<Consulta> Consulta { get; set; } = new List<Consulta>();

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual ICollection<PetVaccination> PetVaccinations { get; set; } = new List<PetVaccination>();

    public virtual ICollection<HistoriaClinica> HistoriasClinicas { get; set; } = new List<HistoriaClinica>();
}
