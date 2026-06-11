using System;

namespace DTOs;

public record CreateMensajeDTO
{
    public int EmisorId { get; set; }

    public int ReceptorId { get; set; }

    public string Contenido { get; set; } = null!;
}
