using System;

namespace vet_api_Net.Models;

public partial class PetVaccination
{
    public int Id { get; set; }
    public int MascotaId { get; set; }
    public int VaccineId { get; set; }
    public int? VaccineBatchId { get; set; }
    public int? ConsultaId { get; set; }
    public int? DoctorId { get; set; }
    public DateOnly ApplicationDate { get; set; }
    public decimal? WeightAtApplication { get; set; }
    public string? Dose { get; set; }
    public DateOnly? NextDoseDate { get; set; }
    public string Status { get; set; } = "Applied";
    public string? PostponementReason { get; set; }
    public string? ClinicalObservations { get; set; }
    public int? DetalleFacturaId { get; set; }
    public DateTime? ReminderSevenSentAt { get; set; }
    public DateTime? ReminderThreeSentAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public virtual Mascota Mascota { get; set; } = null!;
    public virtual Vaccine Vaccine { get; set; } = null!;
    public virtual VaccineBatch? VaccineBatch { get; set; }
    public virtual Consulta? Consulta { get; set; }
    public virtual Usuario? Doctor { get; set; }
    public virtual DetallesFactura? DetalleFactura { get; set; }
}
