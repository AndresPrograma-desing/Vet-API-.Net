using System;

namespace DTOs;

public record CreateClientWithPetDTO
{
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public string? Identificacion { get; set; }
    public string? Nota { get; set; }

    public CreateMascotaDTO Mascota { get; set; } = null!;
}
