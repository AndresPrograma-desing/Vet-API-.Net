using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Repositories;

public interface IPasswordResetTicketRepository
{
    Task<PasswordResetTicket?> GetPendingTicketAsync(int userId);
    Task AddTicketAsync(PasswordResetTicket ticket);
    Task<PasswordResetTicket?> GetTicketByTokenAsync(string token);
    Task<PasswordResetTicket?> GetLatestTicketByUserIdAsync(int userId);
    Task<List<PasswordResetTicket>> GetPendingAndAcceptedTicketsAsync();
    Task<PasswordResetTicket?> GetAcceptedTicketByUserIdAsync(int userId);
    Task UpdateTicketAsync(PasswordResetTicket ticket);
    Task<bool> SaveChangesAsync();
}
