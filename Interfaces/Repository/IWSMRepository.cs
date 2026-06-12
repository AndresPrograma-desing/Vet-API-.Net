using System.Threading.Tasks;
using DTOs;

namespace vet_api_Net.Repositories
{
    public interface IWSMRepository
    {
        Task<WSMessageDbDTO?> GetWSMessageAPIDataAsync();
    }
}