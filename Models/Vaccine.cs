using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace vet_api_Net.Models;

public partial class Vaccine
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public int MinimumAgeWeeks { get; set; }
    public int BoosterFrequencyDays { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public virtual ICollection<VaccineBatch> VaccineBatches { get; set; } = new List<VaccineBatch>();
    public virtual ICollection<PetVaccination> PetVaccinations { get; set; } = new List<PetVaccination>();
}