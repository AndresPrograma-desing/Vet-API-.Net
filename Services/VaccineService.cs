using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Models;

//Describe: Servicio de negocio para el catálogo de vacunas, sus etapas de esquema y sus lotes.
namespace vet_api_Net.Services;

public class VaccineService : IVaccineService
{
    private readonly IVaccineRepository _repository;

    public VaccineService(IVaccineRepository repository)
    {
        _repository = repository;
    }

    public async Task<VaccineListResponseDTO> GetPagedAsync(int pageNumber = 1, int pageSize = 10, string? species = null, bool? active = null, string? searchTerm = null)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(pageNumber, pageSize, species, active, searchTerm);

        return new VaccineListResponseDTO
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = totalCount
        };
    }

    public async Task<VaccineDTO?> GetByIdAsync(int id)
    {
        var vaccine = await _repository.GetByIdWithStagesAsync(id);
        return vaccine == null ? null : MapToDto(vaccine);
    }

    public async Task<VaccineDTO> CreateAsync(VaccineCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateVaccineInput(dto);

        var vaccine = new Vaccine
        {
            Name = dto.Name.Trim(),
            Species = dto.Species.Trim(),
            MinimumAgeWeeks = dto.MinimumAgeWeeks,
            BoosterFrequencyValue = dto.BoosterFrequencyValue,
            BoosterFrequencyUnit = dto.BoosterFrequencyUnit,
            Description = dto.Description,
            Price = dto.Price,
            ProductoId = dto.ProductoId,
            Active = dto.Active,
            CreatedAt = DateTime.Now
        };

        _repository.AddVaccine(vaccine);
        await _repository.SaveChangesAsync();

        return MapToDto(vaccine);
    }

    public async Task<VaccineDTO> UpdateAsync(int id, VaccineCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateVaccineInput(dto);

        var vaccine = await _repository.GetByIdWithStagesAsync(id)
            ?? throw new KeyNotFoundException(ResponseMessagesVaccination.VaccineNotFound);

        vaccine.Name = dto.Name.Trim();
        vaccine.Species = dto.Species.Trim();
        vaccine.MinimumAgeWeeks = dto.MinimumAgeWeeks;
        vaccine.BoosterFrequencyValue = dto.BoosterFrequencyValue;
        vaccine.BoosterFrequencyUnit = dto.BoosterFrequencyUnit;
        vaccine.Description = dto.Description;
        vaccine.Price = dto.Price;
        vaccine.ProductoId = dto.ProductoId;
        vaccine.Active = dto.Active;

        _repository.UpdateVaccine(vaccine);
        await _repository.SaveChangesAsync();

        return MapToDto(vaccine);
    }

    public async Task DeleteAsync(int id)
    {
        var vaccine = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException(ResponseMessagesVaccination.VaccineNotFound);

        _repository.DeleteVaccine(vaccine);
        await _repository.SaveChangesAsync();
    }

    public async Task<VaccineSchemeStageDTO> AddSchemeStageAsync(int vaccineId, VaccineSchemeStageCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var vaccine = await _repository.GetByIdAsync(vaccineId)
            ?? throw new KeyNotFoundException(ResponseMessagesVaccination.VaccineNotFound);

        if (dto.DoseNumber <= 0)
            throw new ArgumentException(ResponseMessagesVaccination.DoseNumberMustBePositive);

        if (dto.IntervalDaysFromPrevious < 0)
            throw new ArgumentException(ResponseMessagesVaccination.IntervalDaysMustBeNonNegative);

        if (dto.StageType != VaccinationVariables.StageInitial && dto.StageType != VaccinationVariables.StageBooster)
            throw new ArgumentException(ResponseMessagesVaccination.InvalidStageType);

        var stage = new VaccineSchemeStage
        {
            VaccineId = vaccine.Id,
            StageType = dto.StageType,
            DoseNumber = dto.DoseNumber,
            IntervalDaysFromPrevious = dto.IntervalDaysFromPrevious,
            MinimumAgeWeeksOverride = dto.MinimumAgeWeeksOverride,
            CreatedAt = DateTime.Now
        };

        _repository.AddSchemeStage(stage);
        await _repository.SaveChangesAsync();

        return new VaccineSchemeStageDTO
        {
            Id = stage.Id,
            VaccineId = stage.VaccineId,
            StageType = stage.StageType,
            DoseNumber = stage.DoseNumber,
            IntervalDaysFromPrevious = stage.IntervalDaysFromPrevious,
            MinimumAgeWeeksOverride = stage.MinimumAgeWeeksOverride
        };
    }

    public async Task RemoveSchemeStageAsync(int vaccineId, int stageId)
    {
        var stage = await _repository.GetSchemeStageByIdAsync(stageId);
        if (stage == null || stage.VaccineId != vaccineId)
            throw new KeyNotFoundException(ResponseMessagesVaccination.SchemeStageNotFound);

        _repository.RemoveSchemeStage(stage);
        await _repository.SaveChangesAsync();
    }

    public async Task<VaccineBatchListResponseDTO> GetBatchesPagedAsync(int vaccineId, int pageNumber = 1, int pageSize = 10, bool? active = null, string? searchTerm = null)
    {
        var (items, totalCount) = await _repository.GetBatchesPagedAsync(vaccineId, pageNumber, pageSize, active, searchTerm);
        var today = DateOnly.FromDateTime(DateTime.Now);

        return new VaccineBatchListResponseDTO
        {
            Items = items.Select(b => MapBatchToDto(b, today)).ToList(),
            TotalCount = totalCount
        };
    }

    public async Task<VaccineBatchDTO> AddBatchAsync(int vaccineId, VaccineBatchCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var vaccine = await _repository.GetByIdAsync(vaccineId)
            ?? throw new KeyNotFoundException(ResponseMessagesVaccination.VaccineNotFound);

        var today = DateOnly.FromDateTime(DateTime.Now);
        if (dto.ExpirationDate <= today)
            throw new ArgumentException(ResponseMessagesVaccination.BatchExpirationMustBeFuture);

        var batch = new VaccineBatch
        {
            VaccineId = vaccine.Id,
            Laboratory = dto.Laboratory.Trim(),
            BatchNumber = dto.BatchNumber.Trim(),
            ExpirationDate = dto.ExpirationDate,
            QuantityInStock = dto.QuantityInStock,
            ReceivedDate = dto.ReceivedDate,
            Active = true,
            CreatedAt = DateTime.Now
        };

        _repository.AddBatch(batch);
        await _repository.SaveChangesAsync();

        return MapBatchToDto(batch, today);
    }

    private static void ValidateVaccineInput(VaccineCreateDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException(ResponseMessagesVaccination.VaccineNameRequired);

        if (string.IsNullOrWhiteSpace(dto.Species))
            throw new ArgumentException(ResponseMessagesVaccination.VaccineSpeciesRequired);

        if (dto.MinimumAgeWeeks < 0)
            throw new ArgumentException(ResponseMessagesVaccination.InvalidMinimumAge);

        if (dto.BoosterFrequencyValue <= 0)
            throw new ArgumentException(ResponseMessagesVaccination.InvalidBoosterFrequency);

        if (dto.BoosterFrequencyUnit != VaccinationVariables.FrequencyDays &&
            dto.BoosterFrequencyUnit != VaccinationVariables.FrequencyMonths &&
            dto.BoosterFrequencyUnit != VaccinationVariables.FrequencyYears)
            throw new ArgumentException(ResponseMessagesVaccination.InvalidBoosterFrequencyUnit);
    }

    private static VaccineDTO MapToDto(Vaccine vaccine) => new()
    {
        Id = vaccine.Id,
        Name = vaccine.Name,
        Species = vaccine.Species,
        MinimumAgeWeeks = vaccine.MinimumAgeWeeks,
        BoosterFrequencyValue = vaccine.BoosterFrequencyValue,
        BoosterFrequencyUnit = vaccine.BoosterFrequencyUnit,
        Description = vaccine.Description,
        Price = vaccine.Price,
        ProductoId = vaccine.ProductoId,
        Active = vaccine.Active,
        CreatedAt = vaccine.CreatedAt,
        SchemeStages = vaccine.SchemeStages?.Select(s => new VaccineSchemeStageDTO
        {
            Id = s.Id,
            VaccineId = s.VaccineId,
            StageType = s.StageType,
            DoseNumber = s.DoseNumber,
            IntervalDaysFromPrevious = s.IntervalDaysFromPrevious,
            MinimumAgeWeeksOverride = s.MinimumAgeWeeksOverride
        }).OrderBy(s => s.DoseNumber).ToList() ?? new()
    };

    private static VaccineBatchDTO MapBatchToDto(VaccineBatch batch, DateOnly today) => new()
    {
        Id = batch.Id,
        VaccineId = batch.VaccineId,
        Laboratory = batch.Laboratory,
        BatchNumber = batch.BatchNumber,
        ExpirationDate = batch.ExpirationDate,
        QuantityInStock = batch.QuantityInStock,
        ReceivedDate = batch.ReceivedDate,
        Active = batch.Active,
        IsExpired = batch.ExpirationDate <= today
    };
}
