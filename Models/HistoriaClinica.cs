using System;

//Describe: Representa la entidad de Historia Clínica de una mascota, consolidando observaciones manuales de veterinarios e insights inteligentes generados por la IA.
namespace vet_api_Net.Models;

public partial class HistoriaClinica
{
    public int Id { get; set; }

    public int MascotaId { get; set; }

    public string? ResumenIa { get; set; }

    public string? AlertasRiesgoIa { get; set; }

    public string? SugerenciasIa { get; set; }

    public string? NotasVeterinario { get; set; }

    public DateTime? UltimoAnalisisIa { get; set; }

    public DateTime Creado { get; set; }

    public DateTime Actualizado { get; set; }

    public virtual Mascota Mascota { get; set; } = null!;
}
