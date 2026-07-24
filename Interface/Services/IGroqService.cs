using System.Threading.Tasks;
using DTOs;

//Describe: Interfaz para el servicio de comunicación directa con Groq para consultas y chats interactivos.
namespace vet_api_Net.Interfaze.Services;

public interface IGroqService
{
    Task<GroqChatResponseDTO> EnviarConsultaAsync(GroqChatRequestDTO dto);
}
