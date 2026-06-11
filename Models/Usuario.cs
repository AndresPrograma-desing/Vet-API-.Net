using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace vet_api_Net.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public required string Email { get; set; } = null!;

    [Column("contraseña")]
    public required string Password { get; set; } = null!;

    public required string Rol { get; set; } = null!;

    public bool? Activo { get; set; }

    public DateTime? UltimoAcceso { get; set; }

    public DateTime Creado { get; set; }

    public DateTime Actualizado { get; set; }

    public virtual ICollection<AlertasInterna> AlertasInternaDestinatarios { get; set; } = new List<AlertasInterna>();

    public virtual ICollection<AlertasInterna> AlertasInternaLeidaPorNavigations { get; set; } = new List<AlertasInterna>();

    public virtual ICollection<Cita> CitaDoctors { get; set; } = new List<Cita>();

    public virtual ICollection<Cita> CitaSecretaria { get; set; } = new List<Cita>();

    public virtual ICollection<Consulta> Consulta { get; set; } = new List<Consulta>();

    public virtual ICollection<ConsultasProducto> ConsultasProductos { get; set; } = new List<ConsultasProducto>();

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual ICollection<HistorialPrecio> HistorialPrecios { get; set; } = new List<HistorialPrecio>();

    public virtual ICollection<LogsSistema> LogsSistemas { get; set; } = new List<LogsSistema>();

    public virtual ICollection<MovimientosInventario> MovimientosInventarios { get; set; } = new List<MovimientosInventario>();

    public virtual ICollection<Vacuna> Vacunas { get; set; } = new List<Vacuna>();

    public virtual ICollection<Mensaje> MensajesEmisor { get; set; } = new List<Mensaje>();

    public virtual ICollection<Mensaje> MensajesReceptor { get; set; } = new List<Mensaje>();
}
