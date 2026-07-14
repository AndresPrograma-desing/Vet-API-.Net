using System.ComponentModel.DataAnnotations;

namespace DTOs;

public class UpdateEmailTemplateDTO
{
    [Required(ErrorMessage = "El código HTML de la plantilla es requerido.")]
    [MinLength(10, ErrorMessage = "El código HTML debe tener al menos 10 caracteres.")]
    public string HtmlCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de correo es requerido.")]
    [StringLength(50, ErrorMessage = "El tipo de correo no puede exceder los 50 caracteres.")]
    public string TypeEmail { get; set; } = string.Empty;
}
