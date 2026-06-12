using System.Threading.Tasks;
using DTOs;

namespace vet_api_Net.Interfaces.Services;

public interface IConsultasService
{
    Task<ConsultaRequestDTO?> CreateConsultaAsync(CreateConsultaDTO dto);
    Task<ConsultaRequestDTO?> GetConsultaByIdAsync(int id);
}