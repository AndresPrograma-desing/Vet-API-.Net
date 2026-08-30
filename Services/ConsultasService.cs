using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Models;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Utilities;
using vet_api_Net.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace vet_api_Net.Services;

public class ConsultasService : IConsultasService
{
    private readonly IConsultasRepository _repository;
    private readonly ApiSettingsOptions _apiSettings;
    private readonly IUserScopeResolver _userScopeResolver;
    private readonly IPetVaccinationService _petVaccinationService;
    public ConsultasService(IConsultasRepository repository, IOptions<ApiSettingsOptions> apiSettingsOptions, IUserScopeResolver userScopeResolver, IPetVaccinationService petVaccinationService)
    {
        _repository = repository;
        _apiSettings = apiSettingsOptions.Value;
        _userScopeResolver = userScopeResolver;
        _petVaccinationService = petVaccinationService;
    }


    public async Task<ConsultaRequestDTO?> CreateConsultaAsync(CreateConsultaDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        // 1. Validaciones de negocio primero
        if (dto.Temperatura > 100)
        {
            throw new ArgumentException(ResponseMessagesConsultas.TemperaturaInvalid);
        }

        var appointment = await _repository.GetCitaByIdAsync(dto.CitaId)
            ?? throw new KeyNotFoundException(ResponseMessagesCitas.CitaNotFound);

        var pet = await _repository.GetMascotaByIdAsync(dto.MascotaId)
            ?? throw new KeyNotFoundException(ResponseMessagesUsers.UserNotFound);

        var doctor = await _repository.GetDoctorByIdAsync(dto.DoctorId)
            ?? throw new KeyNotFoundException(ResponseMessagesUsers.DoctorNotFound);

        var medicalRecord = new Consulta
        {
            CitaId = dto.CitaId,
            MascotaId = dto.MascotaId,
            DoctorId = dto.DoctorId,
            FechaConsulta = dto.FechaConsulta ?? DateTime.Now,
            PesoActual = dto.PesoActual,
            Temperatura = dto.Temperatura,
            Sintomas = dto.Sintomas,
            Diagnostico = dto.Diagnostico,
            Tratamiento = dto.Tratamiento,
            Receta = dto.Receta,
            Observaciones = dto.Observaciones,
            Creado = DateTime.Now,
            ConsultaPrice = dto.ConsultaPrice > 0m ? dto.ConsultaPrice : 0m
        };

        _repository.AddConsulta(medicalRecord);
        await _repository.SaveChangesAsync();

        if (dto.Productos != null && dto.Productos.Any())
        {
            foreach (var p in dto.Productos)
            {
                var product = await _repository.GetProductoByIdAsync(p.ProductoId)
                    ?? throw new KeyNotFoundException($"Producto {p.ProductoId} no encontrado");

                var unitPrice = p.PrecioUnitario.HasValue && p.PrecioUnitario.Value > 0m
                    ? p.PrecioUnitario.Value
                    : (product.PrecioVenta > 0m ? product.PrecioVenta : product.Precio);

                var itemDetail = new ConsultasProducto
                {
                    ConsultaId = medicalRecord.Id,
                    ProductoId = p.ProductoId,
                    Cantidad = p.Cantidad > 0 ? p.Cantidad : 1,
                    PrecioUnitario = unitPrice,
                    Dosis = p.Dosis,
                    ViaAdministracion = p.ViaAdministracion,
                    Frecuencia = p.Frecuencia,
                    Duracion = p.Duracion,
                    Instrucciones = p.Instrucciones,
                    AplicadoPor = p.AplicadoPor,
                    Creado = DateTime.Now
                };
                _repository.AddConsultaProducto(itemDetail);
            }
            await _repository.SaveChangesAsync();
        }

        if (dto.Vacunas != null && dto.Vacunas.Any())
        {
            foreach (var v in dto.Vacunas)
            {
                await _petVaccinationService.RegisterApplicationAsync(new RegisterPetVaccinationDTO
                {
                    MascotaId = dto.MascotaId,
                    VaccineId = v.VaccineId,
                    VaccineBatchId = v.VaccineBatchId,
                    ConsultaId = medicalRecord.Id,
                    DoctorId = dto.DoctorId,
                    ApplicationDate = v.ApplicationDate,
                    WeightAtApplication = v.WeightAtApplication,
                    Dose = v.Dose,
                    NextDoseDateOverride = v.NextDoseDateOverride,
                    ClinicalObservations = v.ClinicalObservations
                });
            }
        }

        await ProcessInternalBillingAsync(medicalRecord.Id, appointment, pet);

        return await _repository.GetConsultaDtoByIdAsync(medicalRecord.Id);
    } 

   public async Task<ConsultaRequestDTO?> GetConsultaByIdAsync(int id)
    {
        return await _repository.GetConsultaDtoByIdAsync(id);
    }

    public async Task<ConsultaPdfDTO?> GetConsultaPdfDataAsync(int id)
    {
        return await _repository.GetConsultaPdfDataAsync(id);
    }

    public async Task<IEnumerable<ConsultaRequestDTO>> GetConsultasAsync(string? currentUserRole = null, int? currentUserId = null, int? requestedDoctorId = null, int? requestedSecretariaId = null)
    {
        var (doctorId, secretariaId) = _userScopeResolver.ResolveDoctorSecretariaScope(currentUserRole, currentUserId, requestedDoctorId, requestedSecretariaId);
        return await _repository.GetConsultasAsync(doctorId, secretariaId);
    }

    public async Task<ConsultaRequestDTO?> UpdateRecetaAsync(int id, UpdateConsultaRecetaDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var consulta = await _repository.GetConsultaByIdEntityAsync(id)
            ?? throw new KeyNotFoundException(ResponseErrors.NotFound);

        consulta.Receta = dto.Receta;
        
        await _repository.SaveChangesAsync();

        return await _repository.GetConsultaDtoByIdAsync(id);
    }

    private async Task ProcessInternalBillingAsync(int recordId, Cita appointment, Mascota pet)
    {
        try
        {
            var targetRecord = await _repository.GetConsultaWithProductsAsync(recordId);
            var hasProductos = targetRecord?.ConsultasProductos != null && targetRecord.ConsultasProductos.Any();
            var pendingVaccinations = targetRecord?.PetVaccinations?.Where(pv => !pv.DetalleFacturaId.HasValue).ToList() ?? new List<PetVaccination>();
            if (targetRecord == null || (!hasProductos && !pendingVaccinations.Any())) return;

            decimal productsSubtotal = hasProductos ? targetRecord.ConsultasProductos.Sum(cp => cp.PrecioUnitario * cp.Cantidad) : 0m;
            decimal vaccinesSubtotal = pendingVaccinations.Sum(pv => pv.Vaccine.Price);
            decimal subtotal = productsSubtotal + vaccinesSubtotal;
            decimal discount = 0m;
            decimal total = subtotal - discount;

            string paymentMethod = "otro";
            if (appointment.MetodoPagoId.HasValue)
            {
                var method = await _repository.GetMetodoPagoByIdAsync(appointment.MetodoPagoId.Value);
                if (method != null && !string.IsNullOrWhiteSpace(method.Nombre))
                {
                    var normalizedName = method.Nombre.ToLower();
                    if (normalizedName.Contains(PaymentMethods.Contain1)) paymentMethod = PaymentMethods.Cash;
                    else if (normalizedName.Contains(PaymentMethods.Contain2)) paymentMethod = PaymentMethods.Card;
                    else if (normalizedName.Contains(PaymentMethods.Contain3)) paymentMethod = PaymentMethods.Transfer;
                    else if (normalizedName.Contains(PaymentMethods.Contain4)) paymentMethod = PaymentMethods.MobilePayment;
                }
            }
            
            var invoice = new Factura
            {
                NumeroFactura = $"R-{DateTime.UtcNow:dd-MM-yyyy}-{recordId}",
                ClienteId = pet.ClienteId,
                MascotaId = pet.Id,
                ConsultaId = recordId,
                SecretariaId = appointment.SecretariaId ?? targetRecord.DoctorId,
                FechaEmision = DateTime.UtcNow,
                Subtotal = subtotal,
                Descuento = discount,
                Total = total,
                MetodoPago = paymentMethod,
                EstadoPago = Status.Pending,
                Notas = targetRecord.Receta ?? targetRecord.Observaciones,
                Creado = DateTime.UtcNow,
                Actualizado = DateTime.UtcNow
            };

            _repository.AddFactura(invoice);
            await _repository.SaveChangesAsync();

            foreach (var cp in targetRecord.ConsultasProductos)
            {
                var resolvedPrice = cp.PrecioUnitario > 0m ? cp.PrecioUnitario : (cp.Producto?.PrecioVenta ?? 0m);
                _repository.AddDetalleFactura(new DetallesFactura
                {
                    FacturaId = invoice.Id,
                    ProductoId = cp.ProductoId,
                    ProductosConsultasId = cp.Id,
                    Descripcion = cp.Producto?.Nombre,
                    Cantidad = cp.Cantidad,
                    PrecioUnitario = resolvedPrice,
                    Total = resolvedPrice * cp.Cantidad,
                    Created = DateTime.UtcNow
                });
            }

            var vaccineDetails = new List<(PetVaccination Vaccination, DetallesFactura Detalle)>();
            foreach (var pv in pendingVaccinations)
            {
                var detalle = new DetallesFactura
                {
                    FacturaId = invoice.Id,
                    VaccineId = pv.VaccineId,
                    Descripcion = pv.Vaccine?.Name,
                    Cantidad = 1,
                    PrecioUnitario = pv.Vaccine?.Price ?? 0m,
                    Total = pv.Vaccine?.Price ?? 0m,
                    Created = DateTime.UtcNow
                };
                _repository.AddDetalleFactura(detalle);
                vaccineDetails.Add((pv, detalle));
            }

            await _repository.SaveChangesAsync();

            foreach (var (pv, detalle) in vaccineDetails)
            {
                pv.DetalleFacturaId = detalle.Id;
            }
            if (vaccineDetails.Any())
                await _repository.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw new Exception(ResponseMessagesFacturaErrors.ErrorProcessingInvoice);
        }
    }
}