using System;
using System.ComponentModel.DataAnnotations;
using vet_api_Net.Constants;

namespace DTOs;

public record CreateUserDTO
{
    [Required(ErrorMessage = ResponseMessagesUsers.NameRequired)]
    [StringLength(100, ErrorMessage = ResponseMessagesDtos.Usuario.NombreMaxLength)]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = ResponseMessagesUsers.NameRequired)]
    [StringLength(100, ErrorMessage = ResponseMessagesDtos.Usuario.ApellidoMaxLength)]
    public string Apellido { get; set; } = null!;

    [Required(ErrorMessage = ResponseMessagesUsers.EmailRequired)]
    [EmailAddress(ErrorMessage = ResponseMessagesDtos.Usuario.InvalidEmailFormat)]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = ResponseMessagesLogin.RequiredPassword)]
    [MinLength(6, ErrorMessage = ResponseMessagesLogin.IsMenorThan6)]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = ResponseMessagesDtos.Usuario.RolRequired)]
    [StringLength(20, ErrorMessage = ResponseMessagesDtos.Usuario.RolMaxLength)]
    public string Rol { get; set; } = null!;

    [StringLength(20, ErrorMessage = ResponseMessagesDtos.Usuario.TelefonoMaxLength)]
    public string? Telefono { get; set; }
}