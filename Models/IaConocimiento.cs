using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vet_api_Net.Models;

[Table("ia_conocimientos")]
public partial class IaConocimiento
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("reglas_respuesta")]
    public string ReglasRespuesta { get; set; } = string.Empty;

    [Column("base_conocimiento")]
    public string BaseConocimiento { get; set; } = string.Empty;

    [Column("categoria")]
    [StringLength(100)]
    public string Categoria { get; set; } = string.Empty;

    [Column("creado")]
    public DateTime Creado { get; set; } = DateTime.Now;

    [Column("actualizado")]
    public DateTime Actualizado { get; set; } = DateTime.Now;
}
