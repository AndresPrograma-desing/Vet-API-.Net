using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaces.Repositories
{
    public interface IMessagesRepository
    {
        Task<List<Mensaje>> GetConversationAsync(int userId, int otherUserId);
        Task<List<Mensaje>> GetUserMessagesAsync(int userId);
        Task<Mensaje?> GetByIdAsync(int messageId);
        void Add(Mensaje mensaje);
        Task<bool> SaveChangesAsync();
        Task<List<Mensaje>> GetUnreadMessagesAsync();
    }
}
