using System;
using System.ComponentModel.DataAnnotations;
using vet_api_Net.Constants;

namespace DTOs;

public record CreateClientWithPetDTO
{
    [Required(ErrorMessage = ResponseMessagesDtos.Cliente.NombreRequired)]
    [StringLength(100, ErrorMessage = ResponseMessagesDtos.Cliente.NombreMaxLength)]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = ResponseMessagesDtos.Cliente.ApellidoRequired)]
    [StringLength(100, ErrorMessage = ResponseMessagesDtos.Cliente.ApellidoMaxLength)]
    public string Apellido { get; set; } = null!;

    [Required(ErrorMessage = ResponseMessagesDtos.Cliente.EmailRequired)]
    [EmailAddress(ErrorMessage = ResponseMessagesDtos.Cliente.InvalidEmailFormat)]
    public string Email { get; set; } = null!;

    [StringLength(20, ErrorMessage = ResponseMessagesDtos.Cliente.TelefonoMaxLength)]
    [RegularExpression(@"^\d+$", ErrorMessage = ResponseMessagesDtos.Cliente.TelefonoInvalidFormat)]
    public string? Telefono { get; set; }

    [StringLength(300, ErrorMessage = ResponseMessagesDtos.Cliente.DireccionMaxLength)]
    public string? Direccion { get; set; }

    [StringLength(20, ErrorMessage = ResponseMessagesDtos.Cliente.IdentificacionMaxLength)]
    public string? Identificacion { get; set; }

    [StringLength(500, ErrorMessage = ResponseMessagesDtos.Cliente.NotaMaxLength)]
    public string? Nota { get; set; }

    [Required]
    public CreateMascotaDTO Mascota { get; set; } = null!;
}
