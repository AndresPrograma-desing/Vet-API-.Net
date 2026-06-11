using System;
using System.Collections.Generic;

namespace vet_api_Net.Models;

public partial class LogsSistema
{
    public int Id { get; set; }

    public int? UsuarioId { get; set; }

    public string Accion { get; set; } = null!;

    public string? TablaAfectada { get; set; }

    public int? RegistroId { get; set; }

    public string? DatosPrevios { get; set; }

    public string? DatosNuevos { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Usuario? Usuario { get; set; }
}
