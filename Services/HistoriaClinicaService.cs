using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Utilities;
using vet_api_Net.Models;

//Describe: Servicio de negocio encargado de coordinar la gestión del historial clínico versionado (1-a-muchos) de las mascotas y su análisis comparativo con Groq.
namespace vet_api_Net.Services;

public class HistoriaClinicaService : IHistoriaClinicaService
{
    private readonly IHistoriaClinicaRepository _repository;
    private readonly IHistoriaClinicaAnalizadorUtilities _analizador;

    public HistoriaClinicaService(IHistoriaClinicaRepository repository, IHistoriaClinicaAnalizadorUtilities analizador)
    {
        _repository = repository;
        _analizador = analizador;
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
        var vacunas = await _repository.GetVacunasByMascotaIdAsync(mascotaId);

        if (hc == null)
        {
            hc = new HistoriaClinica
            {
                MascotaId = mascotaId,
                Creado = DateTime.Now,
                Actualizado = DateTime.Now
            };

            var iaResult = await _analizador.GenerarAnalisisClinicoAsync(mascota, consultas, vacunas, null);
            hc.ResumenIa = iaResult.ResumenIa;
            hc.AlertasRiesgoIa = iaResult.AlertasRiesgoIa;
            hc.SugerenciasIa = iaResult.SugerenciasIa;
            hc.UltimoAnalisisIa = DateTime.Now;

            _repository.Add(hc);
            await _repository.SaveChangesAsync();

            allHcs = new List<HistoriaClinica> { hc };
        }
        else if (hc.UltimoAnalisisIa == null)
        {
            // Caso borde: Existe el registro pero no tiene análisis generado
            var iaResult = await _analizador.GenerarAnalisisClinicoAsync(mascota, consultas, vacunas, null);
            hc.ResumenIa = iaResult.ResumenIa;
            hc.AlertasRiesgoIa = iaResult.AlertasRiesgoIa;
            hc.SugerenciasIa = iaResult.SugerenciasIa;
            hc.UltimoAnalisisIa = DateTime.Now;
            hc.Actualizado = DateTime.Now;

            _repository.Update(hc);
            await _repository.SaveChangesAsync();
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
        var vacunas = await _repository.GetVacunasByMascotaIdAsync(hc.MascotaId);
        var allHcs = await _repository.GetAllByMascotaIdAsync(hc.MascotaId);
        var versionesAnteriores = allHcs.Where(h => h.Id != hc.Id).ToList();

        return MapToResponse(hc, mascota, consultas, vacunas, versionesAnteriores);
    }

    public async Task<HistoriaClinicaResponseDTO?> RefrescarAnalisisIaAsync(int mascotaId)
    {
        var mascota = await _repository.GetMascotaWithDetailsAsync(mascotaId);
        if (mascota == null)
        {
            throw new KeyNotFoundException(ResponseMessagesHistoriaClinica.MascotaNotFound);
        }

        var allHcs = await _repository.GetAllByMascotaIdAsync(mascotaId);
        var latestHc = allHcs.FirstOrDefault();

        var consultas = await _repository.GetConsultasByMascotaIdAsync(mascotaId);
        var vacunas = await _repository.GetVacunasByMascotaIdAsync(mascotaId);

        // Validar si hay nuevos datos clínicos creados después de la fecha del último análisis
        bool hasNewData = latestHc == null || 
                          latestHc.UltimoAnalisisIa == null ||
                          consultas.Any(c => c.FechaConsulta > latestHc.UltimoAnalisisIa.Value) ||
                          vacunas.Any(v => v.FechaVacunacion > DateOnly.FromDateTime(latestHc.UltimoAnalisisIa.Value));

        if (hasNewData)
        {
            // Extraer resumen anterior para el análisis evolutivo de Groq
            string? resumenAnterior = latestHc?.ResumenIa;

            // Generar nuevo análisis por IA
            var iaResult = await _analizador.GenerarAnalisisClinicoAsync(mascota, consultas, vacunas, resumenAnterior);
            
            // Crear nueva historia clínica independiente sin eliminar la anterior
            var newHc = new HistoriaClinica
            {
                MascotaId = mascotaId,
                ResumenIa = iaResult.ResumenIa,
                AlertasRiesgoIa = iaResult.AlertasRiesgoIa,
                SugerenciasIa = iaResult.SugerenciasIa,
                UltimoAnalisisIa = DateTime.Now,
                Creado = DateTime.Now,
                Actualizado = DateTime.Now,
                NotasVeterinario = latestHc?.NotasVeterinario // Mantenemos las últimas notas del veterinario
            };

            _repository.Add(newHc);
            await _repository.SaveChangesAsync();

            // Recargar el listado histórico de historias clínicas
            allHcs = await _repository.GetAllByMascotaIdAsync(mascotaId);
            latestHc = allHcs.FirstOrDefault() ?? newHc;
        }

        var versionesAnteriores = allHcs.Skip(1).ToList();
        return MapToResponse(latestHc!, mascota, consultas, vacunas, versionesAnteriores);
    }

    private HistoriaClinicaResponseDTO MapToResponse(HistoriaClinica hc, Mascota mascota, List<Consulta> consultas, List<Vacuna> vacunas, List<HistoriaClinica> versionesAnteriores)
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
                Especie = mascota.Especie,
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
                Observaciones = c.Observaciones,
                DoctorNombre = c.Doctor != null ? $"{c.Doctor.Nombre} {c.Doctor.Apellido}" : "Veterinario"
            }).ToList(),
            Vacunas = vacunas.Select(v => new HistoriaClinicaVacunaDTO
            {
                Id = v.Id,
                NombreVacuna = v.Producto != null ? v.Producto.Nombre : "Vacuna",
                FechaVacunacion = v.FechaVacunacion,
                ProximaDosis = v.ProximaDosis,
                Lote = v.Lote,
                DoctorNombre = v.Doctor != null ? $"{v.Doctor.Nombre} {v.Doctor.Apellido}" : "Veterinario",
                Nota = v.Nota
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
