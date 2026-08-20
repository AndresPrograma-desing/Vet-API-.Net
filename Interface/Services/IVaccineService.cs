using System.Threading.Tasks;
using DTOs;

namespace vet_api_Net.Interfaze.Services;

public interface IVaccineService
{
    Task<VaccineListResponseDTO> GetPagedAsync(int pageNumber = 1, int pageSize = 10, string? species = null, bool? active = null, string? searchTerm = null);
    Task<VaccineDTO?> GetByIdAsync(int id);
    Task<VaccineDTO> CreateAsync(VaccineCreateDTO dto);
    Task<VaccineDTO> UpdateAsync(int id, VaccineCreateDTO dto);
    Task DeleteAsync(int id);

    Task<VaccineSchemeStageDTO> AddSchemeStageAsync(int vaccineId, VaccineSchemeStageCreateDTO dto);
    Task RemoveSchemeStageAsync(int vaccineId, int stageId);

    Task<VaccineBatchListResponseDTO> GetBatchesPagedAsync(int vaccineId, int pageNumber = 1, int pageSize = 10, bool? active = null, string? searchTerm = null);
    Task<VaccineBatchDTO> AddBatchAsync(int vaccineId, VaccineBatchCreateDTO dto);
}
