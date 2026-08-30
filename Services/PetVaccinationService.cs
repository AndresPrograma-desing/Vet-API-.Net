using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Models;

//Describe: Servicio de negocio para el flujo de aplicación de vacunas: semaforización, registro, postergación, envío a factura y carnet.
namespace vet_api_Net.Services;

public class PetVaccinationService : IPetVaccinationService
{
    private readonly IPetVaccinationRepository _repository;

    public PetVaccinationService(IPetVaccinationRepository repository)
    {
        _repository = repository;
    }

    public async Task<PetVaccinationStatusResponseDTO> GetVaccinationStatusForMascotaAsync(int mascotaId)
    {
        var mascota = await _repository.GetMascotaByIdAsync(mascotaId)
            ?? throw new KeyNotFoundException(ResponseMessagesVaccination.MascotaNotFound);

        var vaccines = await _repository.GetActiveVaccinesByEspecieIdAsync(mascota.EspecieId);
        var today = DateOnly.FromDateTime(DateTime.Now);

        var items = new List<PetVaccinationStatusDTO>();
        foreach (var vaccine in vaccines)
        {
            var lastApplication = await _repository.GetLastApplicationAsync(mascotaId, vaccine.Id);

            items.Add(new PetVaccinationStatusDTO
            {
                VaccineId = vaccine.Id,
                VaccineName = vaccine.Name,
                LastApplicationDate = lastApplication?.ApplicationDate,
                NextDoseDate = lastApplication?.NextDoseDate,
                TrafficLight = ResolveTrafficLight(lastApplication, today)
            });
        }

        return new PetVaccinationStatusResponseDTO
        {
            MascotaId = mascotaId,
            Items = items
        };
    }

    public async Task<PetVaccinationDTO> RegisterApplicationAsync(RegisterPetVaccinationDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var mascota = await _repository.GetMascotaByIdAsync(dto.MascotaId)
            ?? throw new KeyNotFoundException(ResponseMessagesVaccination.MascotaNotFound);

        var vaccine = await _repository.GetVaccineByIdAsync(dto.VaccineId)
            ?? throw new KeyNotFoundException(ResponseMessagesVaccination.VaccineNotFound);

        var batch = await _repository.GetBatchByIdAsync(dto.VaccineBatchId)
            ?? throw new KeyNotFoundException(ResponseMessagesVaccination.BatchNotFound);

        if (batch.VaccineId != vaccine.Id)
            throw new KeyNotFoundException(ResponseMessagesVaccination.BatchNotFound);

        var applicationDate = dto.ApplicationDate ?? DateOnly.FromDateTime(DateTime.Now);

        if (!batch.Active)
            throw new InvalidOperationException(ResponseMessagesVaccination.BatchInactive);

        if (batch.ExpirationDate <= applicationDate)
            throw new InvalidOperationException(ResponseMessagesVaccination.BatchExpired);

        if (batch.QuantityInStock <= 0)
            throw new InvalidOperationException(ResponseMessagesVaccination.BatchNoStock);

        Usuario? doctor = null;
        if (dto.DoctorId.HasValue)
        {
            doctor = await _repository.GetDoctorByIdAsync(dto.DoctorId.Value);
        }

        var nextDoseDate = dto.NextDoseDateOverride ?? await ResolveNextDoseDateAsync(vaccine, dto.MascotaId, applicationDate);

        var petVaccination = new PetVaccination
        {
            MascotaId = dto.MascotaId,
            VaccineId = vaccine.Id,
            VaccineBatchId = batch.Id,
            ConsultaId = dto.ConsultaId,
            DoctorId = dto.DoctorId,
            ApplicationDate = applicationDate,
            WeightAtApplication = dto.WeightAtApplication,
            Dose = dto.Dose,
            NextDoseDate = nextDoseDate,
            Status = VaccinationVariables.StatusApplied,
            ClinicalObservations = dto.ClinicalObservations,
            CreatedAt = DateTime.Now
        };

        batch.QuantityInStock -= 1;
        _repository.UpdateBatch(batch);
        _repository.AddPetVaccination(petVaccination);
        await _repository.SaveChangesAsync();

        return MapToDto(petVaccination, vaccine, batch, doctor);
    }

    public async Task<PetVaccinationDTO> PostponeAsync(int id, PostponeVaccinationDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (string.IsNullOrWhiteSpace(dto.Reason))
            throw new ArgumentException(ResponseMessagesVaccination.PostponementReasonRequired);

        var petVaccination = await _repository.GetByIdWithDetailsAsync(id)
            ?? throw new KeyNotFoundException(ResponseMessagesVaccination.PetVaccinationNotFound);

        if (petVaccination.Status == VaccinationVariables.StatusPostponed)
            throw new InvalidOperationException(ResponseMessagesVaccination.AlreadyPostponed);

        petVaccination.Status = VaccinationVariables.StatusPostponed;
        petVaccination.PostponementReason = dto.Reason.Trim();

        _repository.UpdatePetVaccination(petVaccination);
        await _repository.SaveChangesAsync();

        return MapToDto(petVaccination, petVaccination.Vaccine, petVaccination.VaccineBatch, petVaccination.Doctor);
    }

    public async Task<PetVaccinationDTO> SendToInvoiceAsync(int id)
    {
        var petVaccination = await _repository.GetByIdWithDetailsAsync(id)
            ?? throw new KeyNotFoundException(ResponseMessagesVaccination.PetVaccinationNotFound);

        if (petVaccination.DetalleFacturaId.HasValue)
            throw new InvalidOperationException(ResponseMessagesVaccination.AlreadySentToInvoice);

        if (!petVaccination.ConsultaId.HasValue)
            throw new InvalidOperationException(ResponseMessagesVaccination.NoConsultaLinked);

        var factura = await _repository.GetFacturaByConsultaIdAsync(petVaccination.ConsultaId.Value)
            ?? throw new KeyNotFoundException(ResponseMessagesVaccination.FacturaNotFoundForConsulta);

        var detalle = new DetallesFactura
        {
            FacturaId = factura.Id,
            VaccineId = petVaccination.Vaccine.Id,
            Descripcion = petVaccination.Vaccine.Name,
            Cantidad = 1,
            PrecioUnitario = petVaccination.Vaccine.Price,
            Total = petVaccination.Vaccine.Price,
            Created = DateTime.Now
        };

        _repository.AddDetalleFactura(detalle);
        await _repository.SaveChangesAsync();

        factura.Subtotal += petVaccination.Vaccine.Price;
        factura.Total += petVaccination.Vaccine.Price;
        factura.Actualizado = DateTime.Now;
        _repository.UpdateFactura(factura);

        petVaccination.DetalleFacturaId = detalle.Id;
        _repository.UpdatePetVaccination(petVaccination);

        await _repository.SaveChangesAsync();

        return MapToDto(petVaccination, petVaccination.Vaccine, petVaccination.VaccineBatch, petVaccination.Doctor);
    }

    public async Task<List<PetVaccinationDTO>> GetHistoryForMascotaAsync(int mascotaId, string? status = null)
    {
        var mascota = await _repository.GetMascotaByIdAsync(mascotaId)
            ?? throw new KeyNotFoundException(ResponseMessagesVaccination.MascotaNotFound);

        var history = await _repository.GetHistoryByMascotaIdAsync(mascotaId, status);

        return history.Select(v => MapToDto(v, v.Vaccine, v.VaccineBatch, v.Doctor)).ToList();
    }

    public async Task<PetVaccinationCarnetPdfDTO?> GetCarnetDataAsync(int mascotaId)
    {
        var mascota = await _repository.GetMascotaByIdAsync(mascotaId);
        if (mascota == null) return null;

        var history = await _repository.GetHistoryByMascotaIdAsync(mascotaId);

        return new PetVaccinationCarnetPdfDTO
        {
            MascotaId = mascota.Id,
            MascotaNombre = mascota.Nombre,
            Especie = mascota.Especie.Nombre,
            Raza = mascota.Raza,
            ClienteNombre = mascota.Cliente != null ? $"{mascota.Cliente.Nombre} {mascota.Cliente.Apellido}" : string.Empty,
            ClienteTelefono = mascota.Cliente?.Telefono,
            Items = history.Select(v => new PetVaccinationCarnetItemDTO
            {
                VaccineName = v.Vaccine.Name,
                ApplicationDate = v.ApplicationDate,
                BatchNumber = v.VaccineBatch?.BatchNumber,
                Laboratory = v.VaccineBatch?.Laboratory,
                NextDoseDate = v.NextDoseDate,
                DoctorName = v.Doctor != null ? $"{v.Doctor.Nombre} {v.Doctor.Apellido}" : null
            }).ToList()
        };
    }

    public async Task<PendingVaccinationListResponseDTO> GetPendingThisWeekAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
    {
        var (items, totalCount) = await _repository.GetPendingThisWeekAsync(pageNumber, pageSize, searchTerm);

        return new PendingVaccinationListResponseDTO
        {
            Items = items.Select(v => new PendingVaccinationDTO
            {
                PetVaccinationId = v.Id,
                MascotaId = v.MascotaId,
                MascotaNombre = v.Mascota.Nombre,
                ClienteNombre = v.Mascota.Cliente != null ? $"{v.Mascota.Cliente.Nombre} {v.Mascota.Cliente.Apellido}" : string.Empty,
                ClienteTelefono = v.Mascota.Cliente?.Telefono,
                VaccineName = v.Vaccine.Name,
                NextDoseDate = v.NextDoseDate!.Value
            }).ToList(),
            TotalCount = totalCount
        };
    }

    private async Task<DateOnly> ResolveNextDoseDateAsync(Vaccine vaccine, int mascotaId, DateOnly applicationDate)
    {
        var stages = await _repository.GetSchemeStagesByVaccineIdAsync(vaccine.Id);
        var appliedCount = await _repository.CountAppliedDosesAsync(mascotaId, vaccine.Id);
        var nextDoseNumber = appliedCount + 1;

        var stage = stages.FirstOrDefault(s => s.StageType == VaccinationVariables.StageInitial && s.DoseNumber == nextDoseNumber);
        if (stage != null)
        {
            return applicationDate.AddDays(stage.IntervalDaysFromPrevious);
        }

        return AddFrequency(applicationDate, vaccine.BoosterFrequencyValue, vaccine.BoosterFrequencyUnit);
    }

    private static DateOnly AddFrequency(DateOnly date, int value, string unit) => unit switch
    {
        VaccinationVariables.FrequencyMonths => date.AddMonths(value),
        VaccinationVariables.FrequencyYears => date.AddYears(value),
        _ => date.AddDays(value)
    };

    private static string ResolveTrafficLight(PetVaccination? lastApplication, DateOnly today)
    {
        if (lastApplication == null)
            return VaccinationVariables.TrafficLightRed;

        if (lastApplication.NextDoseDate == null)
            return VaccinationVariables.TrafficLightGreen;

        var daysUntil = lastApplication.NextDoseDate.Value.DayNumber - today.DayNumber;

        if (daysUntil < 0)
            return VaccinationVariables.TrafficLightRed;

        return daysUntil <= VaccinationVariables.AlertWindowDays
            ? VaccinationVariables.TrafficLightYellow
            : VaccinationVariables.TrafficLightGreen;
    }

    private static PetVaccinationDTO MapToDto(PetVaccination v, Vaccine vaccine, VaccineBatch? batch, Usuario? doctor) => new()
    {
        Id = v.Id,
        MascotaId = v.MascotaId,
        VaccineId = v.VaccineId,
        VaccineName = vaccine.Name,
        VaccineBatchId = v.VaccineBatchId,
        BatchNumber = batch?.BatchNumber,
        ConsultaId = v.ConsultaId,
        DoctorId = v.DoctorId,
        DoctorName = doctor != null ? $"{doctor.Nombre} {doctor.Apellido}" : null,
        ApplicationDate = v.ApplicationDate,
        WeightAtApplication = v.WeightAtApplication,
        Dose = v.Dose,
        NextDoseDate = v.NextDoseDate,
        Status = v.Status,
        PostponementReason = v.PostponementReason,
        ClinicalObservations = v.ClinicalObservations,
        DetalleFacturaId = v.DetalleFacturaId
    };
}
