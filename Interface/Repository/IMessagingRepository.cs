using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using vet_api_Net.Models;

//Describe: Contrato de acceso a datos para los mensajes internos entre usuarios.
namespace vet_api_Net.Interfaze.Repositories;

public interface IMessagingRepository
{
    Task<Mensaje?> GetByIdAsync(int id);
    Task<List<Mensaje>> GetConversationAsync(int userId, int otherUserId);
    Task<List<Mensaje>> GetRecentConversationAsync(int userId, int otherUserId, int take);
    Task<List<Mensaje>> GetByReceptorAsync(int userId);
    Task<List<Mensaje>> GetUnreadAsync(CancellationToken cancellationToken = default);
    Task<bool> TableExistsAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Mensaje mensaje);
    Task<bool> SaveChangesAsync();
}
