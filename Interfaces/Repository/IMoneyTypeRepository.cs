using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaces.Repositories
{
    public interface IMoneyTypeRepository
    {
        Task<MoneyType?> GetFirstOrDefaultAsync();
        Task<MoneyType?> GetDollarPersistenceAsync();
        void Add(MoneyType moneyType);
        void Update(MoneyType moneyType);
        Task<bool> SaveChangesAsync();
    }
}
