using System;
using System.Collections.Generic;

namespace vet_api_Net.Models;

public partial class PermissionModule
{
    public int Id { get; set; }

    public string ModuleKey { get; set; } = null!;

    public string Label { get; set; } = null!;

    public string Icon { get; set; } = null!;

    public int SortOrder { get; set; }

    public DateTime Creado { get; set; }

    public virtual ICollection<PermissionDefinition> Permissions { get; set; } = new List<PermissionDefinition>();
}
