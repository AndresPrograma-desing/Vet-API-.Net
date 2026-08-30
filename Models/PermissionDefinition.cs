using System;

namespace vet_api_Net.Models;

public partial class PermissionDefinition
{
    public int Id { get; set; }

    public int ModuleId { get; set; }

    public string Key { get; set; } = null!;

    public string Label { get; set; } = null!;

    public int SortOrder { get; set; }

    public DateTime Creado { get; set; }

    public virtual PermissionModule Module { get; set; } = null!;
}
