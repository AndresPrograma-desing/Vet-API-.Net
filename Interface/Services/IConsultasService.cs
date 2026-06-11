using System.Threading.Tasks;
using DTOs;

namespace vet_api_Net.Interfaze.Services;

public interface IConsultasService
{
    Task<ConsultaRequestDTO?> CreateConsultaAsync(CreateConsultaDTO dto);
    Task<ConsultaRequestDTO?> GetConsultaByIdAsync(int id);
}