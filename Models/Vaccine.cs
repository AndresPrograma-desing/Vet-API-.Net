using System;
using System.Collections.Generic;

namespace vet_api_Net.Models;

public partial class Vaccine
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public int MinimumAgeWeeks { get; set; }
    public int BoosterFrequencyValue { get; set; }
    public string BoosterFrequencyUnit { get; set; } = "days";
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int? ProductoId { get; set; }
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public virtual Producto? Producto { get; set; }
    public virtual ICollection<VaccineSchemeStage> SchemeStages { get; set; } = new List<VaccineSchemeStage>();
    public virtual ICollection<VaccineBatch> Batches { get; set; } = new List<VaccineBatch>();
    public virtual ICollection<PetVaccination> PetVaccinations { get; set; } = new List<PetVaccination>();
}
