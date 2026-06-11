using System.Threading.Tasks;
using DTOs;

namespace vet_api_Net.Services.WSMessage;

public interface IWSMessage
{
    Task<bool> EnviarComprobanteAsync(WSMessageDTO datosEnvio);
    Task<bool> IniciarSesionAsync();
    Task<WSStatusResponseDTO?> ObtenerEstadoOSesionAsync();
}