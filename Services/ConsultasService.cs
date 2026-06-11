using System;
using System.Linq;
using System.Threading.Tasks;
using DTOs;
using vet_api_Net.Models;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;

namespace vet_api_Net.Services;

public class ConsultasService : IConsultasService
{
    private readonly IConsultasRepository _repository;

    public ConsultasService(IConsultasRepository repository)
    {
        _repository = repository;
    }

    public async Task<ConsultaRequestDTO?> CreateConsultaAsync(CreateConsultaDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        try
        {
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

            await ProcessInternalBillingAsync(medicalRecord.Id, appointment, pet);

            return await _repository.GetConsultaDtoByIdAsync(medicalRecord.Id);
        }
        catch (KeyNotFoundException ex)
        {
            throw new InvalidOperationException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al crear la consulta: {ex.Message}");
        }
    }

    public async Task<ConsultaRequestDTO?> GetConsultaByIdAsync(int id)
    {
        return await _repository.GetConsultaDtoByIdAsync(id);
    }

    private async Task ProcessInternalBillingAsync(int recordId, Cita appointment, Mascota pet)
    {
        try
        {
            var targetRecord = await _repository.GetConsultaWithProductsAsync(recordId);
            if (targetRecord?.ConsultasProductos == null || !targetRecord.ConsultasProductos.Any()) return;

            decimal subtotal = targetRecord.ConsultasProductos.Sum(cp => cp.PrecioUnitario * cp.Cantidad);
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
                NumeroFactura = $"F-{DateTime.UtcNow:yyyyMMddHHmmss}-{recordId}",
                ClienteId = pet.ClienteId,
                MascotaId = pet.Id,
                ConsultaId = recordId,
                SecretariaId = appointment.SecretariaId ?? targetRecord.DoctorId,
                FechaEmision = DateTime.UtcNow,
                Subtotal = subtotal,
                Descuento = discount,
                Total = total,
                MetodoPago = paymentMethod,
                EstadoPago = "pendiente",
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
            await _repository.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw new Exception(ResponseMessagesFacturaErrors.ErrorProcessingInvoice);
        }
    }
}