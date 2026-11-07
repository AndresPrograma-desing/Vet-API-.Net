using DTOs;

//Describe: Contrato del servicio para consultar y actualizar el horario laboral del sistema.
namespace vet_api_Net.Interfaze.Services;

public interface IWorkingHoursService
{
    Task<WorkingHoursDTO> GetAsync();
    Task<WorkingHoursDTO> UpdateAsync(WorkingHoursDTO dto);
}
