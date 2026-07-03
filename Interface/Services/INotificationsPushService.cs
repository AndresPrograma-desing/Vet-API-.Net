using System.Threading.Tasks;
using DTOs; 

namespace vet_api_Net.Interfaze.Services;

public interface INotificationsPushService
{
    // Método para enviar el push usando el Id del destinatario detectado
    Task SendPushToUserAsync(int targetUserId, string message, int alertId);
    // Metodo para recibir cualquier tipo de datos para notificaciones
    Task SendNotificationAsync(PushNotificationDTO data);
    
}