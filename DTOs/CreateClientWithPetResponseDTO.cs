namespace DTOs;

public record CreateClientWithPetResponseDTO
{
    public int ClienteId { get; set; }
    public int MascotaId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
}
