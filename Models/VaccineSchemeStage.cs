using System;

namespace vet_api_Net.Models;

public partial class VaccineSchemeStage
{
    public int Id { get; set; }
    public int VaccineId { get; set; }
    public string StageType { get; set; } = string.Empty;
    public int DoseNumber { get; set; }
    public int IntervalDaysFromPrevious { get; set; }
    public int? MinimumAgeWeeksOverride { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public virtual Vaccine Vaccine { get; set; } = null!;
}
