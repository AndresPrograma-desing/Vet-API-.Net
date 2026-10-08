using System.Threading;
using System.Threading.Tasks;
using vet_api_Net.Models;

//Describe: Contrato de acceso a datos para la tabla de monedas y la tasa BCV.
namespace vet_api_Net.Interfaze.Repositories;

public interface IMoneyTypeRepository
{
    Task<MoneyType?> GetFirstAsync();
    Task<MoneyType?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<MoneyType?> GetByDollarPersistenceAsync(string dollarPersistence);
    void Add(MoneyType moneyType);
    void Update(MoneyType moneyType);
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}
