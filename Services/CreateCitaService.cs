using System;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Interfaces.Services;
using vet_api_Net.Interfaces.Repositories;
using vet_api_Net.Models;
using vet_api_Net.Constants;

namespace vet_api_Net.Services
{
    public class CreateCitaService : ICreateCitaService
    {
        private readonly ICitasRepository _citasRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IPetsRepository _petsRepository;

        public CreateCitaService(ICitasRepository citasRepository, IUsersRepository usersRepository, IPetsRepository petsRepository)
        {
            _citasRepository = citasRepository;
            _usersRepository = usersRepository;
            _petsRepository = petsRepository;
        }

        public async Task<Cita> CreateCitaAsync(CreateCitaDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            if (dto.MascotaId <= 0) throw new ArgumentException(ResponseMessagesCitas.InvalidMascotaId);
            if (dto.DoctorId <= 0) throw new ArgumentException(ResponseMessagesUsers.DoctorNotFound);
            if (string.IsNullOrWhiteSpace(dto.HoraCita)) throw new ArgumentException(ResponseMessagesCitas.RequiredHoraCita);

            var mascota = await _petsRepository.GetMascotaByIdWithClienteAsync(dto.MascotaId);
            if (mascota == null) throw new KeyNotFoundException(ResponseMessagesCitas.MascotaNotFound);

            var doctor = await _usersRepository.GetByIdAsync(dto.DoctorId);
            if (doctor == null) throw new KeyNotFoundException(ResponseMessagesUsers.DoctorNotFound);

            if (dto.SecretariaId.HasValue)
            {
                var sec = await _usersRepository.GetByIdAsync(dto.SecretariaId.Value);
                if (sec == null) throw new KeyNotFoundException(ResponseMessagesUsers.SecretarialNotFound);
            }

            TimeOnly hora;
            try
            {
                hora = TimeOnly.Parse(dto.HoraCita);
            }
            catch (Exception)
            {
                throw new ArgumentException(ResponseMessagesCitas.InvalidHoraCita);
            }

            // --- MANEJO ROBUSTO DE LA FECHA ---
            DateTime fecha = dto.FechaCita.HasValue ? dto.FechaCita.Value.Date : DateTime.Now.Date;

            var conflict = await _citasRepository.AnyConflictAsync(dto.DoctorId, fecha, hora);
            if (conflict) throw new InvalidOperationException(ResponseMessagesCitas.ExistingCitaConflict);

            var cita = new Cita
            {
                MascotaId = dto.MascotaId,
                DoctorId = dto.DoctorId,
                SecretariaId = dto.SecretariaId,
                FechaCita = fecha,
                HoraCita = hora,
                Motivo = dto.Motivo,
                TipoCita = dto.TipoCita ?? TypeConsultas.Consulta,
                Estado = dto.Estado ?? Status.Programed,
                Notas = dto.Notas
            };

            if (!string.IsNullOrWhiteSpace(dto.MetodoPago))
            {
                var metodoNombre = dto.MetodoPago!.Trim();
                var metodo = await _citasRepository.GetMetodoPagoByNameAsync(metodoNombre);
                if (metodo == null)
                {
                    metodo = new MetodoPago
                    {
                        Nombre = metodoNombre,
                        Creado = DateTime.Now,
                        Actualizado = DateTime.Now
                    };
                    _citasRepository.AddMetodoPago(metodo);
                    await _citasRepository.SaveChangesAsync();
                }

                cita.MetodoPagoId = metodo.Id;
            }

            await _citasRepository.AddAsync(cita);
            await _citasRepository.SaveChangesAsync();

            var citaRecargada = await _citasRepository.GetByIdWithMetodoPagoAsync(cita.Id);
            return citaRecargada ?? cita;
        }
    }
}