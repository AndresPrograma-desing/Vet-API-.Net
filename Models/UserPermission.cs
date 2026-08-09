using System;

namespace vet_api_Net.Models;

public partial class UserPermission
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Permissions { get; set; } = "[]";

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Usuario User { get; set; } = null!;
}
