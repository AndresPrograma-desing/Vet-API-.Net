using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;

//Describe: Contrato para el repositorio de Historia Clínica, definiendo las operaciones de acceso a datos para la persistencia.
namespace vet_api_Net.Interfaze.Repositories;

public interface IHistoriaClinicaRepository
{
    Task<HistoriaClinica?> GetByMascotaIdAsync(int mascotaId);
    Task<List<HistoriaClinica>> GetAllByMascotaIdAsync(int mascotaId);
    Task<HistoriaClinica?> GetByIdAsync(int id);
    void Add(HistoriaClinica historiaClinica);
    void Update(HistoriaClinica historiaClinica);
    Task<bool> SaveChangesAsync();
    Task<Mascota?> GetMascotaWithDetailsAsync(int mascotaId);
    Task<List<Consulta>> GetConsultasByMascotaIdAsync(int mascotaId);
    Task<List<Vacuna>> GetVacunasByMascotaIdAsync(int mascotaId);
}
