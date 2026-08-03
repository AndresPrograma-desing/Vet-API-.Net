// Interfaz para el servicio de negocio de correos electrónicos. Define los métodos para enviar correos basándose en plantillas configuradas según el tipo de correo.
using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;

namespace vet_api_Net.Interfaze.Services;

public interface IEmailService
{
    Task<InvoiceDispatchResponseDTO> DispatchEmailAsync(int entityId, string typeEmail);
    Task<IEnumerable<EmailsResponsesDTO>> GetAllEmailTemplatesAsync();
    Task<EmailsResponsesDTO?> GetEmailTemplateByIdAsync(int id);
    Task<EmailsResponsesDTO?> UpdateEmailTemplateAsync(int id, UpdateEmailTemplateDTO dto);
    Task<DataResendDto> GetResendConfigAsync();
    Task UpdateResendConfigAsync(DataResendDto dto);
}