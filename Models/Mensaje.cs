using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace vet_api_Net.Models;

public partial class Mensaje
{
    public int Id { get; set; }

    public int EmisorId { get; set; }

    public int ReceptorId { get; set; }

    public string Contenido { get; set; } = null!;

    public bool? Leido { get; set; }

    public DateTime FechaEnvio { get; set; }

    public virtual Usuario Emisor { get; set; } = null!;

    public virtual Usuario Receptor { get; set; } = null!;
}
