using System.Threading.Tasks;
using DTOs;

//Describe: Interfaz para el servicio de negocio que gestiona la Historia Clínica e integra el análisis inteligente.
namespace vet_api_Net.Interfaze.Services;

public interface IHistoriaClinicaService
{
    Task<HistoriaClinicaResponseDTO?> GetHistoriaClinicaByMascotaIdAsync(int mascotaId);
    Task<HistoriaClinicaResponseDTO?> UpdateNotasClinicasAsync(int id, UpdateHistoriaClinicaDTO dto);
    Task<HistoriaClinicaResponseDTO?> RefrescarAnalisisIaAsync(int mascotaId);
}
