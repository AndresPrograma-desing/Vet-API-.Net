using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaces.Repositories;

public interface IClientPetRepository
{
    Task<bool> ExistsByEmailAsync(string email);
    Task<IDbContextTransaction> BeginTransactionAsync();
    void AddClient(Cliente client);
    void AddPet(Mascota pet);
    Task<bool> SaveChangesAsync();
    Task<bool> ClientExistsAsync(int clientId);
    Task<IEnumerable<Cliente>> GetClientsWithPetsLookupAsync(string searchTerm);
    Task<bool> PetExistsByIdentificacionAsync(string identificacion);
}