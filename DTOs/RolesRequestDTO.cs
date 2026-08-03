namespace DTOs;

public record RolesRequestDTO
{
    public List<string> Roles { get; set; } = new();
}