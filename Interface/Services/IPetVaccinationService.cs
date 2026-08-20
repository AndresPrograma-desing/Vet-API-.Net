using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;

namespace vet_api_Net.Interfaze.Services;

public interface IPetVaccinationService
{
    Task<PetVaccinationStatusResponseDTO> GetVaccinationStatusForMascotaAsync(int mascotaId);
    Task<PetVaccinationDTO> RegisterApplicationAsync(RegisterPetVaccinationDTO dto);
    Task<PetVaccinationDTO> PostponeAsync(int id, PostponeVaccinationDTO dto);
    Task<PetVaccinationDTO> SendToInvoiceAsync(int id);
    Task<List<PetVaccinationDTO>> GetHistoryForMascotaAsync(int mascotaId, string? status = null);
    Task<PetVaccinationCarnetPdfDTO?> GetCarnetDataAsync(int mascotaId);
    Task<PendingVaccinationListResponseDTO> GetPendingThisWeekAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null);
}
