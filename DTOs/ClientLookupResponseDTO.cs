namespace DTOs;

public record ClientLookupResponseDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Identificacion { get; set; } = string.Empty;
    public List<PetLookupDTO> Mascotas { get; set; } = new();
}

public record PetLookupDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}