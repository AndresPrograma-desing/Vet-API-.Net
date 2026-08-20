using System;
using System.Collections.Generic;

namespace vet_api_Net.Models;

public partial class VaccineBatch
{
    public int Id { get; set; }
    public int VaccineId { get; set; }
    public string Laboratory { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
    public DateOnly ExpirationDate { get; set; }
    public int QuantityInStock { get; set; }
    public DateOnly ReceivedDate { get; set; }
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public virtual Vaccine Vaccine { get; set; } = null!;
    public virtual ICollection<PetVaccination> PetVaccinations { get; set; } = new List<PetVaccination>();
}
