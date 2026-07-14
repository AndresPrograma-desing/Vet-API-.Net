using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace vet_api_Net.Models;

public partial class PasswordResetTicket
{
    public int Id { get; set; }

    [Column("usuario_id")]
    public int UsuarioId { get; set; }

    [Column("token")]
    public string Token { get; set; } = null!;

    [Column("estado")]
    public string Estado { get; set; } = null!;

    [Column("expiracion")]
    public DateTime Expiracion { get; set; }

    [Column("creado")]
    public DateTime Creado { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}
