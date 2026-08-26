using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DTOs;
using vet_api_Net.Models;
using vet_api_Net.Constants;
using vet_api_Net.Infrastructure.Configuration;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Utilities;
using vet_api_Net.Utilities;

namespace vet_api_Net.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IFacturasRepository _facturasRepository;
    private readonly ICitasRequestService _citasRequestService;
    private readonly ILogger<InvoiceService> _logger;
    private readonly ICurrencyUtilities _currencyService;
    private readonly ApiSettingsOptions _apiSettings;

    public InvoiceService(
        IFacturasRepository facturasRepository, 
        ICitasRequestService citasRequestService, 
        ILogger<InvoiceService> logger, 
        ICurrencyUtilities currencyService, 
        IOptions<ApiSettingsOptions> apiSettingsOptions)
    {
        _facturasRepository = facturasRepository;
        _citasRequestService = citasRequestService;
        _logger = logger;
        _currencyService = currencyService;
        _apiSettings = apiSettingsOptions.Value;
    }

    private async Task<List<FacturationItemDTO>> MapearItemsAsync(Consulta? consulta, List<DetallesFactura>? detalles, decimal convertedConsultaPrice)
    {
        var items = new List<FacturationItemDTO>();
        var cps = consulta?.ConsultasProductos?.ToList() ?? new List<ConsultasProducto>();

        if (cps.Count > 0)
        {
            foreach (var cp in cps)
            {
                var basePrice = cp.PrecioUnitario > 0m ? cp.PrecioUnitario : (cp.Producto?.PrecioVenta ?? cp.Producto?.Precio ?? 0m);
                decimal unitPrice = await _currencyService.ConvertPriceAsync(basePrice);

                items.Add(new FacturationItemDTO
                {
                    ProductosConsultasId = cp.Id,
                    ProductoId = cp.ProductoId,
                    Codigo = cp.Producto?.Codigo ?? string.Empty,
                    Nombre = cp.Producto?.Nombre ?? string.Empty,
                    Descripcion = cp.Producto?.Descripcion,
                    PrecioUnitario = unitPrice,
                    Cantidad = cp.Cantidad,
                    Total = unitPrice * cp.Cantidad,
                    ConsultaPrice = convertedConsultaPrice
                });
            }
        }
        else if (detalles != null && detalles.Count > 0)
        {
            foreach (var d in detalles)
            {
                decimal unitPrice = await _currencyService.ConvertPriceAsync(d.PrecioUnitario);
                items.Add(new FacturationItemDTO
                {
                    ProductosConsultasId = d.ProductosConsultasId,
                    ProductoId = d.ProductoId,
                    VaccineId = d.VaccineId,
                    Codigo = d.Producto?.Codigo ?? string.Empty,
                    Nombre = d.Producto?.Nombre ?? d.Vaccine?.Name ?? string.Empty,
                    Descripcion = d.Descripcion ?? d.Producto?.Descripcion,
                    PrecioUnitario = unitPrice,
                    Cantidad = d.Cantidad,
                    Total = unitPrice * d.Cantidad,
                    ConsultaPrice = convertedConsultaPrice
                });
            }
        }

        return items;
    }

    public async Task<FacturationDTO?> GenerateInvoiceForCitaAsync(int citaId)
    {
        var cita = await _facturasRepository.GetCitaWithClienteAsync(citaId);
        if (cita == null) return null;
        if (cita.Estado != Status.Completed) throw new InvalidOperationException(ResponseMessagesFacturaErrors.OnlyCitaAllowed);

        Consulta? consulta = await _facturasRepository.GetConsultaForInvoiceAsync(citaId, cita.MascotaId);
        if (consulta == null) return null;

        decimal convertedConsultaPrice = await _currencyService.ConvertPriceAsync(consulta.ConsultaPrice ?? 0m);
        var detalles = await _facturasRepository.GetDetallesByConsultaIdAsync(consulta.Id);

        var items = await MapearItemsAsync(consulta, detalles, convertedConsultaPrice);
        var subtotal = items.Sum(i => i.Total);
        decimal descuento = 0m;

        string metodoPagoCalculado = PaymentMethods.Others;
        if (cita.MetodoPagoId != null)
        {
            var metodo = await _facturasRepository.GetMetodoPagoByIdAsync(cita.MetodoPagoId.Value);
            if (metodo != null && !string.IsNullOrWhiteSpace(metodo.Nombre))
            {
                var name = metodo.Nombre.Trim().ToLowerInvariant();
                metodoPagoCalculado = name switch
                {
                    _ when name.Contains(PaymentMethods.Contain1) => PaymentMethods.Cash,
                    _ when name.Contains(PaymentMethods.Contain2) || name.Contains(PaymentMethods.Contain3) => PaymentMethods.Card,
                    _ when name.Contains(PaymentMethods.Contain4) => PaymentMethods.Transfer,
                    _ when name.Contains(PaymentMethods.Contain5) || name.Contains(PaymentMethods.Contain6) => PaymentMethods.MobilePayment,
                    _ => PaymentMethods.Others
                };
            }
        }

        string estadoPagoCalculado = Status.Pending;
        var facturaDb = await _facturasRepository.GetFacturaByConsultaIdAsync(consulta.Id);
        string numeroFacturaCalculado = $"TMP-{DateTime.UtcNow:yyyyMMddHHmmss}-{consulta.Id}";
        string? urlDocxCalculada = null;

        if (facturaDb != null)
        {
            if (!string.IsNullOrWhiteSpace(facturaDb.NumeroFactura)) numeroFacturaCalculado = facturaDb.NumeroFactura;
            if (!string.IsNullOrWhiteSpace(facturaDb.EstadoPago)) estadoPagoCalculado = facturaDb.EstadoPago;
            if (!string.IsNullOrWhiteSpace(facturaDb.MetodoPago)) metodoPagoCalculado = facturaDb.MetodoPago;
            if (!string.IsNullOrWhiteSpace(facturaDb.UrlDocx)) urlDocxCalculada = facturaDb.UrlDocx;
        }

        var dto = new FacturationDTO
        {
            NumeroFactura = numeroFacturaCalculado,
            ClienteId = consulta.Mascota?.ClienteId ?? cita.Mascota.ClienteId,
            ClienteNombre = consulta.Mascota?.Cliente != null ? $"{consulta.Mascota.Cliente.Nombre} {consulta.Mascota.Cliente.Apellido}" : (cita.Mascota.Cliente != null ? $"{cita.Mascota.Cliente.Nombre} {cita.Mascota.Cliente.Apellido}" : null),
            MascotaId = consulta.Mascota?.Id ?? cita.MascotaId,
            MascotaNombre = consulta.Mascota?.Nombre ?? cita.Mascota?.Nombre,
            ConsultaId = consulta.Id,
            DoctorId = consulta.DoctorId,
            DoctorNombre = consulta.Doctor != null ? $"{consulta.Doctor.Nombre} {consulta.Doctor.Apellido}" : null,
            FechaEmision = DateTime.UtcNow,
            Items = items,
            Subtotal = subtotal,
            Descuento = descuento,
            Total = subtotal + convertedConsultaPrice - descuento,
            MetodoPago = metodoPagoCalculado,
            EstadoPago = estadoPagoCalculado,
            Notas = consulta.Receta ?? consulta.Observaciones,
            Creado = DateTime.UtcNow,
            Actualizado = DateTime.UtcNow,
            UrlDocx = urlDocxCalculada
        };

        return dto;
    }

    public async Task SaveOrUpdateFacturaPersistenceAsync(int citaId, int consultaId, string numeroFactura, string urlDocx, FacturationDTO dto)
    {
        var facturaExistente = await _facturasRepository.GetFacturaByConsultaIdAsync(consultaId);

        if (facturaExistente != null)
        {
            facturaExistente.UrlDocx = urlDocx;
            facturaExistente.NumeroFactura = numeroFactura;
            facturaExistente.EstadoPago = dto.EstadoPago;
            facturaExistente.MetodoPago = dto.MetodoPago;
            facturaExistente.Actualizado = DateTime.UtcNow;
            
            _facturasRepository.UpdateFactura(facturaExistente);
            await _facturasRepository.SaveChangesAsync();
        }
        else
        {
            var citaEntity = await _facturasRepository.GetCitaByIdAsync(citaId);
            int secretariaId = 0;

            if (citaEntity?.SecretariaId != null && citaEntity.SecretariaId > 0)
                secretariaId = citaEntity.SecretariaId.Value;
            else if (dto.DoctorId.HasValue && dto.DoctorId.Value > 0)
                secretariaId = dto.DoctorId.Value;
            else
                secretariaId = await _facturasRepository.GetFirstUsuarioIdAsync();

            if (secretariaId > 0)
            {
                var consulta = await _facturasRepository.GetConsultaForInvoiceAsync(citaId, citaEntity?.MascotaId ?? dto.MascotaId!.Value);
                
                decimal subtotalInDb = 0m;
                decimal consultaPriceInDb = consulta?.ConsultaPrice ?? 0m;

                if (consulta?.ConsultasProductos != null)
                {
                    foreach (var cp in consulta.ConsultasProductos)
                    {
                        var basePrice = cp.PrecioUnitario > 0m ? cp.PrecioUnitario : (cp.Producto?.PrecioVenta ?? cp.Producto?.Precio ?? 0m);
                        subtotalInDb += (basePrice * cp.Cantidad);
                    }
                }

                decimal totalInDb = subtotalInDb + consultaPriceInDb;

                var nueva = new Factura
                {
                    NumeroFactura = numeroFactura,
                    ClienteId = dto.ClienteId,
                    MascotaId = dto.MascotaId,
                    ConsultaId = consultaId,
                    SecretariaId = secretariaId,
                    FechaEmision = DateTime.UtcNow,
                    Subtotal = subtotalInDb,
                    Descuento = dto.Descuento ?? 0m,
                    Total = totalInDb - (dto.Descuento ?? 0m),
                    MetodoPago = dto.MetodoPago,
                    EstadoPago = dto.EstadoPago == Status.InvoicePending ? Status.Pending : dto.EstadoPago,
                    Notas = dto.Notas,
                    Creado = DateTime.UtcNow,
                    Actualizado = DateTime.UtcNow,
                    UrlDocx = urlDocx
                };

                _facturasRepository.AddFactura(nueva);
                await _facturasRepository.SaveChangesAsync();
            }
        }
    }

    public async Task<List<FacturationDTO>> GenerateInvoicesForClientAsync(int clienteId, string targetCurrency = "USD")
    {
        var petIds = await _facturasRepository.GetMascotaIdsByClienteIdAsync(clienteId);
        if (petIds == null || petIds.Count == 0) return new List<FacturationDTO>();

        var consultas = await _facturasRepository.GetConsultasWithRelationsAsync(petIds);
        var results = new List<FacturationDTO>();

        foreach (var consulta in consultas)
        {
            decimal convertedConsultaPrice = await _currencyService.ConvertPriceAsync(consulta.ConsultaPrice ?? 0m);
            var detalles = await _facturasRepository.GetDetallesByConsultaIdAsync(consulta.Id);

            var items = await MapearItemsAsync(consulta, detalles, convertedConsultaPrice);
            var subtotal = items.Sum(i => i.Total);
            decimal descuento = 0m;

            string metodoPagoCalculado = PaymentMethods.Others;
            string estadoPagoCalculado = Status.Pending;
            string numeroFacturaCalculado = $"TMP-{DateTime.UtcNow:yyyyMMddHHmmss}-{consulta.Id}";
            string? urlDocxCalculada = null;

            var facturaDb = await _facturasRepository.GetFacturaByConsultaIdAsync(consulta.Id);
            if (facturaDb != null)
            {
                if (!string.IsNullOrWhiteSpace(facturaDb.NumeroFactura)) numeroFacturaCalculado = facturaDb.NumeroFactura;
                if (!string.IsNullOrWhiteSpace(facturaDb.EstadoPago)) estadoPagoCalculado = facturaDb.EstadoPago;
                if (!string.IsNullOrWhiteSpace(facturaDb.MetodoPago)) metodoPagoCalculado = facturaDb.MetodoPago;
                if (!string.IsNullOrWhiteSpace(facturaDb.UrlDocx)) urlDocxCalculada = facturaDb.UrlDocx;
            }

            results.Add(new FacturationDTO
            {
                NumeroFactura = numeroFacturaCalculado,
                ClienteId = consulta.Mascota?.ClienteId ?? 0,
                ClienteNombre = consulta.Mascota?.Cliente != null ? $"{consulta.Mascota.Cliente.Nombre} {consulta.Mascota.Cliente.Apellido}" : null,
                MascotaId = consulta.Mascota?.Id,
                MascotaNombre = consulta.Mascota?.Nombre,
                ConsultaId = consulta.Id,
                DoctorId = consulta.DoctorId,
                DoctorNombre = consulta.Doctor != null ? $"{consulta.Doctor.Nombre} {consulta.Doctor.Apellido}" : null,
                FechaEmision = DateTime.UtcNow,
                Items = items,
                Subtotal = subtotal,
                Descuento = descuento,
                Total = subtotal + convertedConsultaPrice - descuento,
                MetodoPago = metodoPagoCalculado,
                EstadoPago = estadoPagoCalculado,
                Notas = consulta.Receta ?? consulta.Observaciones,
                Creado = DateTime.UtcNow,
                Actualizado = DateTime.UtcNow,
                UrlDocx = urlDocxCalculada
            });
        }

        return results;
    }

 public async Task<List<FacturationDTO>> AllFacturasAsync()
{
    var facturas = await _facturasRepository.GetAllFacturasWithRelationsAsync();
    if (facturas == null || facturas.Count == 0)
        return new List<FacturationDTO>();

    int? idMasReciente = facturas.FirstOrDefault()?.Id;

    List<FacturationDTO> results = new List<FacturationDTO>();
    foreach (var factura in facturas)
    {
        var consulta = factura.Consulta;
        decimal convertedConsultaPrice = await _currencyService.ConvertPriceAsync(consulta?.ConsultaPrice ?? 0m);
        var detalles = await _facturasRepository.GetDetallesByFacturaIdAsync(factura.Id);
        var items = await MapearItemsAsync(consulta, detalles, convertedConsultaPrice);

        if (!items.Any())
        {
            items.Add(new FacturationItemDTO
            {
                ProductoId = ResponseMessagesFactura.EmptyProductId,
                Codigo = ResponseMessagesFactura.NA,
                Nombre = ResponseMessagesFactura.EmptyProductName,
                Descripcion = ResponseMessagesFactura.EmptyProductDescription,
                PrecioUnitario = ResponseMessagesFactura.EmptyProductPrice,
                Cantidad = ResponseMessagesFactura.EmptyProductQuantity,
                Total = ResponseMessagesFactura.EmptyProductTotal,
                ConsultaPrice = convertedConsultaPrice
            });
        }

        var subtotal = items.Sum(i => i.Total);
        decimal descuento = factura.Descuento ?? 0m;
        results.Add(new FacturationDTO
        {
            FacturaId = factura.Id,
            NumeroFactura = factura.NumeroFactura,
            ClienteId = factura.ClienteId,
            ClienteNombre = consulta?.Mascota?.Cliente != null ? $"{consulta.Mascota.Cliente.Nombre} {consulta.Mascota.Cliente.Apellido}" : null,
            MascotaId = factura.MascotaId,
            MascotaNombre = consulta?.Mascota?.Nombre,
            ConsultaId = factura.ConsultaId,
            CitaId = consulta?.CitaId,
            DoctorId = consulta?.DoctorId,
            DoctorNombre = consulta?.Doctor != null ? $"{consulta.Doctor.Nombre} {consulta.Doctor.Apellido}" : null,
            FechaEmision = factura.FechaEmision,
            Items = items,
            Subtotal = subtotal,
            Descuento = descuento,
            Total = subtotal + convertedConsultaPrice - descuento,
            MetodoPago = factura.MetodoPago,
            EstadoPago = factura.EstadoPago,
            Notas = consulta?.Receta ?? consulta?.Observaciones ?? string.Empty,
            Creado = factura.Creado,
            Actualizado = factura.Actualizado,
            UrlDocx = factura.UrlDocx,
            IsNew = (factura.Id == idMasReciente)
        });
    }

    return results;
}

    public async Task<FacturationDTO?> ChangeStatusFacturaAsync(int facturaId, string estadoPago, string? metodoPago = null)
    {
        var factura = await _facturasRepository.GetFacturaByIdAsync(facturaId);
        if (factura == null) return null;

        factura.EstadoPago = string.IsNullOrWhiteSpace(estadoPago) ? factura.EstadoPago : estadoPago;
        if (!string.IsNullOrWhiteSpace(metodoPago)) factura.MetodoPago = metodoPago;
        factura.Actualizado = DateTime.UtcNow;

        _facturasRepository.UpdateFactura(factura);
        await _facturasRepository.SaveChangesAsync();

        var consulta = factura.Consulta;
        decimal convertedConsultaPrice = await _currencyService.ConvertPriceAsync(consulta?.ConsultaPrice ?? 0m);
        var detalles = await _facturasRepository.GetDetallesByFacturaIdAsync(factura.Id);
        var items = await MapearItemsAsync(consulta, detalles, convertedConsultaPrice);

        var subtotal = items.Sum(i => i.Total);
        decimal descuento = factura.Descuento ?? 0m;

        return new FacturationDTO
        {
            FacturaId = factura.Id,
            NumeroFactura = factura.NumeroFactura,
            ClienteId = factura.ClienteId,
            ClienteNombre = consulta?.Mascota?.Cliente != null ? $"{consulta.Mascota.Cliente.Nombre} {consulta.Mascota.Cliente.Apellido}" : null,
            MascotaId = factura.MascotaId,
            MascotaNombre = consulta?.Mascota?.Nombre,
            ConsultaId = factura.ConsultaId,
            DoctorId = consulta?.DoctorId,
            DoctorNombre = consulta?.Doctor != null ? $"{consulta.Doctor.Nombre} {consulta.Doctor.Apellido}" : null,
            FechaEmision = factura.FechaEmision,
            Items = items,
            Subtotal = subtotal,
            Descuento = descuento,
            Total = subtotal + convertedConsultaPrice - descuento,
            MetodoPago = factura.MetodoPago,
            EstadoPago = factura.EstadoPago,
            Notas = factura.Notas ?? ResponseMessagesFactura.Notes,
            Creado = factura.Creado,
            Actualizado = factura.Actualizado,
            UrlDocx = factura.UrlDocx
        };
    }

    public async Task<FacturaSettingDTO?> GetFacturaSettingsAsync()
    {
        var fc = await _facturasRepository.GetFirstFacturaConfigAsync();
        if (fc == null) return null;

        return new FacturaSettingDTO
        {
            Days = fc.RetentionValue ?? 30,
            IsEnabled = fc.IsEnabled,
            GenerateEnabled = fc.GenerateEnabled ?? false,
            LastUpdated = fc.LastUpdated
        };
    }
}