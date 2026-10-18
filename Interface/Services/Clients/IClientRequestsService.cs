using System.Threading.Tasks;
using DTOs;

namespace vet_api_Net.Interfaze.Services.Clients;

public interface IClientRequestsService
{
    Task CancelCitaAsync(int clienteId, int citaId, string motivo);
    Task ConfirmCitaAsync(int clienteId, int citaId, string? mensaje);
    Task RequestCitaAsync(int clienteId, ClientRequestCitaDTO dto);
    Task RequestRecetaRenewalAsync(int clienteId, int consultaId, string nota);
}
