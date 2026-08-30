using System.Collections.Generic;

namespace vet_api_Net.Models;

public partial class Especie
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Mascota> Mascotas { get; set; } = new List<Mascota>();

    public virtual ICollection<Vaccine> Vaccines { get; set; } = new List<Vaccine>();
}
