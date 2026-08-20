using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Repositories;

public interface IVaccineRepository
{
    Task<(List<Vaccine> Items, int TotalCount)> GetPagedAsync(int pageNumber = 1, int pageSize = 10, string? species = null, bool? active = null, string? searchTerm = null);
    Task<Vaccine?> GetByIdAsync(int id);
    Task<Vaccine?> GetByIdWithStagesAsync(int id);
    void AddVaccine(Vaccine vaccine);
    void UpdateVaccine(Vaccine vaccine);
    void DeleteVaccine(Vaccine vaccine);

    Task<VaccineSchemeStage?> GetSchemeStageByIdAsync(int id);
    void AddSchemeStage(VaccineSchemeStage stage);
    void RemoveSchemeStage(VaccineSchemeStage stage);

    Task<(List<VaccineBatch> Items, int TotalCount)> GetBatchesPagedAsync(int vaccineId, int pageNumber, int pageSize, bool? active = null, string? searchTerm = null);
    Task<VaccineBatch?> GetBatchByIdAsync(int id);
    void AddBatch(VaccineBatch batch);
    void UpdateBatch(VaccineBatch batch);

    Task<bool> SaveChangesAsync();
}
