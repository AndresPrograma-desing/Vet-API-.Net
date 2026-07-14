// Interfaz para el canal de envío de correos electrónicos. Define el contrato para enviar correos, pudiendo incluir archivos adjuntos.
using System.Threading.Tasks;

namespace vet_api_Net.Interfaze.Services;

public interface IEmailSenderService
{
    // Soporta envío normal y adjuntos (como los PDFs de facturas)
    Task<bool> SendEmailAsync(string to, string subject, string htmlBody, byte[]? attachmentBytes = null, string? attachmentName = null);
}
