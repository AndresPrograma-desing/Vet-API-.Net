using System;
using System.Globalization;
using DTOs;
using vet_api_Net.Models;

namespace vet_api_Net.Extensions;

public static class CitaMappingExtensions
{
    public static CitasRequestDTO ToRequestDTO(this Cita cr)
    {
        return new CitasRequestDTO
        {
            Id = cr.Id,
            MascotaId = cr.MascotaId,
            DoctorId = cr.DoctorId,
            SecretariaId = cr.SecretariaId,
            FechaCita = DateTime.SpecifyKind(cr.FechaCita, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ss.fff'Z'"),
            HoraCita = cr.HoraCita.ToString("HH:mm:ss"),
            Motivo = cr.Motivo,
            TipoCita = cr.TipoCita ?? string.Empty,
            Estado = cr.Estado ?? string.Empty,
            Notas = cr.Notas,
            MetodoPagoId = cr.MetodoPagoId,
            MetodoPago = cr.MetodoPago?.Nombre,
            Mascotum = cr.Mascota != null ? new MascotaResumenDTO
            {
                Id = cr.Mascota.Id,
                ClienteId = cr.Mascota.ClienteId,
                Nombre = cr.Mascota.Nombre,
                Especie = cr.Mascota.Especie,
                Raza = cr.Mascota.Raza,
                Sexo = cr.Mascota.Sexo,
                FechaNacimiento = cr.Mascota.FechaNacimiento?.ToString("yyyy-MM-dd"),
                Peso = cr.Mascota.Peso?.ToString("0.00", CultureInfo.InvariantCulture),
                IdenteficacionMascota = cr.Mascota.IdenteficacionMascota,
                Cliente = cr.Mascota.Cliente != null ? new DetailsCitaDTO
                {
                    Id = cr.Mascota.Cliente.Id,
                    Nombre = cr.Mascota.Cliente.Nombre,
                    Apellido = cr.Mascota.Cliente.Apellido,
                    Email = cr.Mascota.Cliente.Email,
                    Telefono = cr.Mascota.Cliente.Telefono,
                    Identificacion = cr.Mascota.Cliente.Identificacion
                } : null!
            } : null,
            Doctor = cr.Doctor != null ? new UsuarioResumenDTO
            {
                Id = cr.Doctor.Id,
                Nombre = cr.Doctor.Nombre,
                Apellido = cr.Doctor.Apellido,
                Rol = cr.Doctor.Rol
            } : null,
            Secretaria = cr.Secretaria != null ? new UsuarioResumenDTO
            {
                Id = cr.Secretaria.Id,
                Nombre = cr.Secretaria.Nombre,
                Apellido = cr.Secretaria.Apellido,
                Rol = cr.Secretaria.Rol
            } : null
        };
    }
}