using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Models;
using vet_api_Net.Interfaces.Repositories;

namespace vet_api_Net.Repositories
{
    public class MoneyTypeRepository : IMoneyTypeRepository
    {
        private readonly AppDbContext _context;

        public MoneyTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MoneyType?> GetFirstOrDefaultAsync()
        {
            return await _context.MoneyTypes.FirstOrDefaultAsync();
        }

        public async Task<MoneyType?> GetDollarPersistenceAsync()
        {
            return await _context.MoneyTypes.FirstOrDefaultAsync(m => m.DollarPersistence == "USD");
        }

        public void Add(MoneyType moneyType)
        {
            _context.MoneyTypes.Add(moneyType);
        }

        public void Update(MoneyType moneyType)
        {
            _context.MoneyTypes.Update(moneyType);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
