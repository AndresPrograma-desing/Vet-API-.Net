using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Models;

//Describe: Acceso a datos de la tabla de monedas y la tasa BCV.
namespace vet_api_Net.Repositories;

public class MoneyTypeRepository : IMoneyTypeRepository
{
    private readonly AppDbContext _context;

    public MoneyTypeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MoneyType?> GetFirstAsync()
        => await _context.MoneyTypes.FirstOrDefaultAsync();

    public async Task<MoneyType?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.MoneyTypes.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task<MoneyType?> GetByDollarPersistenceAsync(string dollarPersistence)
        => await _context.MoneyTypes.FirstOrDefaultAsync(m => m.DollarPersistence == dollarPersistence);

    public void Add(MoneyType moneyType)
        => _context.MoneyTypes.Add(moneyType);

    public void Update(MoneyType moneyType)
        => _context.MoneyTypes.Update(moneyType);

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken) > 0;
}
