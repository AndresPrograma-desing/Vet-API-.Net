using System;

namespace vet_api_Net.Models;

public partial class IntegrationCredential
{
    public int Id { get; set; }

    public string Provider { get; set; } = null!;

    public string SecretValue { get; set; } = null!;

    public string? Label { get; set; }

    public int DailyLimit { get; set; }

    public int UsageCount { get; set; }

    public DateOnly UsageDate { get; set; }

    public bool IsActive { get; set; }

    public DateTime Creado { get; set; }

    public DateTime Actualizado { get; set; }
}
