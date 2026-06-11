using System;

namespace DTOs;

public record MensajeDTO
{
    public int Id { get; set; }

    public int EmisorId { get; set; }

    public int ReceptorId { get; set; }

    public string Contenido { get; set; } = null!;

    public bool Leido { get; set; }

    public DateTime FechaEnvio { get; set; }
}
