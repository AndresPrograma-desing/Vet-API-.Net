using System;
using System.Collections.Generic;

namespace vet_api_Net.Models;

public partial class AlertasInterna
{
    public int Id { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Mensaje { get; set; }

    public string Tipo { get; set; } = null!;

    public string? Prioridad { get; set; }

    public string? DestinatarioRol { get; set; }

    public int? DestinatarioId { get; set; }

    public string? ReferenciaTabla { get; set; }

    public int? ReferenciaId { get; set; }

    public bool? AccionRequerida { get; set; }

    public DateOnly? FechaLimite { get; set; }

    public bool? Completada { get; set; }

    public DateTime? FechaCompletado { get; set; }

    public bool? Leida { get; set; }

    public DateTime? FechaLectura { get; set; }

    public int? LeidaPor { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Usuario? Destinatario { get; set; }

    public virtual Usuario? LeidaPorNavigation { get; set; }
}
