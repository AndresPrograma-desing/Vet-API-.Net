using DTOs;
using System.Collections.Generic;

namespace vet_api_Net.Interfaces.Services;

public interface IMessagingService
{
    Task<MensajeDTO> SendMessageAsync(CreateMensajeDTO dto);

    Task<List<MensajeDTO>> GetConversationAsync(int userId, int otherUserId);

    Task<List<MensajeDTO>> GetUserMessagesAsync(int userId);

    Task MarkAsReadAsync(int messageId);
}
