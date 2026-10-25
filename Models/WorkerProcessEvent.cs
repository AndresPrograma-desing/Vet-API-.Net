using System;

namespace vet_api_Net.Models;

public partial class WorkerProcessEvent
{
    public int Id { get; set; }

    public string NameEvent { get; set; } = null!;

    public string TypeEvent { get; set; } = null!;

    public string? Title { get; set; }

    public string? ErrorMessage { get; set; }

    public string? VinculationData { get; set; }

    public string? Snapshot { get; set; }

    public int Intents { get; set; }

    public bool IsResolved { get; set; }

    public DateTime Created { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }
}
