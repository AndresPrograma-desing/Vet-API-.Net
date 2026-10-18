using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Models;

//Describe: Servicio de negocio encargado de coordinar la gestión del historial clínico versionado (1-a-muchos) de las mascotas. El análisis de IA ya no se genera aquí: lo mantiene HistoriaClinicaSummaryWorker en lote contra Gemini.
namespace vet_api_Net.Services;

public class HistoriaClinicaService : IHistoriaClinicaService
{
    private readonly IHistoriaClinicaRepository _repository;
    private readonly IPetsRepository _petsRepository;

    public HistoriaClinicaService(IHistoriaClinicaRepository repository, IPetsRepository petsRepository)
    {
        _repository = repository;
        _petsRepository = petsRepository;
    }

    public async Task<HistoriaClinicaResponseDTO?> GetHistoriaClinicaByMascotaIdAsync(int mascotaId)
    {
        var mascota = await _repository.GetMascotaWithDetailsAsync(mascotaId);
        if (mascota == null)
        {
            throw new KeyNotFoundException(ResponseMessagesHistoriaClinica.MascotaNotFound);
        }

        var allHcs = await _repository.GetAllByMascotaIdAsync(mascotaId);
        var hc = allHcs.FirstOrDefault();
        var consultas = await _repository.GetConsultasByMascotaIdAsync(mascotaId);
        var vacunas = await _repository.GetPetVaccinationsByMascotaIdAsync(mascotaId);

        if (hc == null)
        {
            // Sin resumen generado todavía: HistoriaClinicaSummaryWorker la tomará en su próximo ciclo (Mascota sin HistoriaClinica cuenta como pendiente).
            hc = new HistoriaClinica
            {
                MascotaId = mascotaId,
                Creado = DateTime.Now,
                Actualizado = DateTime.Now
            };

            _repository.Add(hc);
            await _repository.SaveChangesAsync();

            allHcs = new List<HistoriaClinica> { hc };
        }

        var versionesAnteriores = allHcs.Skip(1).ToList();
        return MapToResponse(hc, mascota, consultas, vacunas, versionesAnteriores);
    }

    public async Task<HistoriaClinicaResponseDTO?> UpdateNotasClinicasAsync(int id, UpdateHistoriaClinicaDTO dto)
    {
        var hc = await _repository.GetByIdAsync(id);
        if (hc == null)
        {
            throw new KeyNotFoundException(ResponseMessagesHistoriaClinica.HistoriaClinicaNotFound);
        }

        var mascota = await _repository.GetMascotaWithDetailsAsync(hc.MascotaId);
        if (mascota == null)
        {
            throw new KeyNotFoundException(ResponseMessagesHistoriaClinica.MascotaNotFound);
        }

        hc.NotasVeterinario = dto.NotasVeterinario;
        hc.Actualizado = DateTime.Now;

        _repository.Update(hc);
        await _repository.SaveChangesAsync();

        var consultas = await _repository.GetConsultasByMascotaIdAsync(hc.MascotaId);
        var vacunas = await _repository.GetPetVaccinationsByMascotaIdAsync(hc.MascotaId);
        var allHcs = await _repository.GetAllByMascotaIdAsync(hc.MascotaId);
        var versionesAnteriores = allHcs.Where(h => h.Id != hc.Id).ToList();

        return MapToResponse(hc, mascota, consultas, vacunas, versionesAnteriores);
    }

    // No llama a Gemini directamente: solo marca a la mascota como pendiente (LastDataModifiedAt = ahora)
    // para que HistoriaClinicaSummaryWorker la tome y genere el resumen en su próximo ciclo por lote.
    public async Task<HistoriaClinicaResponseDTO?> RefrescarAnalisisIaAsync(int mascotaId)
    {
        var mascota = await _repository.GetMascotaWithDetailsAsync(mascotaId);
        if (mascota == null)
        {
            throw new KeyNotFoundException(ResponseMessagesHistoriaClinica.MascotaNotFound);
        }

        await _petsRepository.TouchLastModifiedAsync(mascotaId);

        var allHcs = await _repository.GetAllByMascotaIdAsync(mascotaId);
        var latestHc = allHcs.FirstOrDefault();
        var consultas = await _repository.GetConsultasByMascotaIdAsync(mascotaId);
        var vacunas = await _repository.GetPetVaccinationsByMascotaIdAsync(mascotaId);

        if (latestHc == null)
        {
            latestHc = new HistoriaClinica
            {
                MascotaId = mascotaId,
                Creado = DateTime.Now,
                Actualizado = DateTime.Now
            };

            _repository.Add(latestHc);
            await _repository.SaveChangesAsync();

            allHcs = new List<HistoriaClinica> { latestHc };
        }

        var versionesAnteriores = allHcs.Skip(1).ToList();
        return MapToResponse(latestHc, mascota, consultas, vacunas, versionesAnteriores);
    }

    private HistoriaClinicaResponseDTO MapToResponse(HistoriaClinica hc, Mascota mascota, List<Consulta> consultas, List<PetVaccination> vacunas, List<HistoriaClinica> versionesAnteriores)
    {
        return new HistoriaClinicaResponseDTO
        {
            Id = hc.Id,
            MascotaId = hc.MascotaId,
            ResumenIa = hc.ResumenIa,
            AlertasRiesgoIa = hc.AlertasRiesgoIa,
            SugerenciasIa = hc.SugerenciasIa,
            NotasVeterinario = hc.NotasVeterinario,
            UltimoAnalisisIa = hc.UltimoAnalisisIa,
            Creado = hc.Creado,
            Actualizado = hc.Actualizado,
            Mascota = new HistoriaClinicaMascotaDTO
            {
                Nombre = mascota.Nombre,
                Especie = mascota.Especie.Nombre,
                Raza = mascota.Raza,
                Sexo = mascota.Sexo,
                FechaNacimiento = mascota.FechaNacimiento,
                Peso = mascota.Peso,
                Alergias = mascota.Alergias,
                CondicionesMedicas = mascota.CondicionesMedicas
            },
            Consultas = consultas.Select(c => new HistoriaClinicaConsultaDTO
            {
                Id = c.Id,
                FechaConsulta = c.FechaConsulta,
                Sintomas = c.Sintomas,
                Diagnostico = c.Diagnostico,
                Tratamiento = c.Tratamiento,
                Receta = c.Receta,
                UrlDocx = c.UrlDocx,
                Observaciones = c.Observaciones,
                DoctorNombre = c.Doctor != null ? $"{c.Doctor.Nombre} {c.Doctor.Apellido}" : "Veterinario"
            }).ToList(),
            Vacunas = vacunas.Select(v => new HistoriaClinicaVacunaDTO
            {
                Id = v.Id,
                NombreVacuna = v.Vaccine != null ? v.Vaccine.Name : "Vacuna",
                FechaVacunacion = v.ApplicationDate,
                ProximaDosis = v.NextDoseDate,
                Lote = v.VaccineBatch?.BatchNumber,
                DoctorNombre = v.Doctor != null ? $"{v.Doctor.Nombre} {v.Doctor.Apellido}" : "Veterinario",
                Nota = v.ClinicalObservations
            }).ToList(),
            VersionesAnteriores = versionesAnteriores.Select(v => new HistoriaClinicaVersionDTO
            {
                Id = v.Id,
                ResumenIa = v.ResumenIa,
                AlertasRiesgoIa = v.AlertasRiesgoIa,
                SugerenciasIa = v.SugerenciasIa,
                Creado = v.Creado
            }).ToList()
        };
    }
}
