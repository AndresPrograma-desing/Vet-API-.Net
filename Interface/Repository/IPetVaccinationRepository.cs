using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;

namespace vet_api_Net.Interfaze.Repositories;

public interface IPetVaccinationRepository
{
    Task<Mascota?> GetMascotaByIdAsync(int id);
    Task<Usuario?> GetDoctorByIdAsync(int id);
    Task<Vaccine?> GetVaccineByIdAsync(int id);
    Task<VaccineBatch?> GetBatchByIdAsync(int id);
    Task<List<VaccineSchemeStage>> GetSchemeStagesByVaccineIdAsync(int vaccineId);
    Task<List<Vaccine>> GetActiveVaccinesByEspecieIdAsync(int especieId);
    Task<int> CountAppliedDosesAsync(int mascotaId, int vaccineId);
    Task<PetVaccination?> GetLastApplicationAsync(int mascotaId, int vaccineId);

    Task<PetVaccination?> GetByIdAsync(int id);
    Task<PetVaccination?> GetByIdWithDetailsAsync(int id);
    void AddPetVaccination(PetVaccination petVaccination);
    void UpdatePetVaccination(PetVaccination petVaccination);
    void UpdateBatch(VaccineBatch batch);

    Task<List<PetVaccination>> GetHistoryByMascotaIdAsync(int mascotaId, string? status = null);
    Task<(List<PetVaccination> Items, int TotalCount)> GetPendingThisWeekAsync(int pageNumber, int pageSize, string? searchTerm = null);
    Task<List<PetVaccination>> GetDueForReminderAsync(DateOnly sevenDayTarget, DateOnly threeDayTarget);
    Task<List<PetVaccination>> GetDueForDayBeforeReminderAsync(DateOnly dayBeforeTarget);

    Task<Factura?> GetFacturaByConsultaIdAsync(int consultaId);
    void AddDetalleFactura(DetallesFactura detalle);
    void UpdateFactura(Factura factura);

    Task<bool> SaveChangesAsync();
}
