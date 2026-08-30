using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using DTOs;
using vet_api_Net.Constants;
using vet_api_Net.Infrastructure.Configuration;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Services.WSMessage;
using vet_api_Net.Repositories;

namespace vet_api_Net.Services;

public class InvoiceExternalService : IInvoiceExternalService
{
    private readonly IInvoiceExternalRepository _repository;
    private readonly IWSMessage _wsMessageService;
    private readonly ApiSettingsOptions _apiSettings;
    private readonly IWSMRepository _wsmRepository;

    public InvoiceExternalService(IInvoiceExternalRepository repository, IWSMessage wsMessageService, IOptions<ApiSettingsOptions> apiSettingsOptions, IWSMRepository wsmRepository)
    {
        _repository = repository;
        _wsMessageService = wsMessageService;
        _apiSettings = apiSettingsOptions.Value;
        _wsmRepository = wsmRepository;
    }

    public async Task<InvoiceDispatchResponseDTO> VerifyAndDispatchInvoiceByCitaAsync(int citaId)
    {
        var cita = await _repository.GetCitaWithDetailsAsync(citaId)
            ?? throw new KeyNotFoundException(ResponseMessagesWSMessageAPI.CitaNotFound);

        if (cita.Estado != Status.Completed)
        {
            throw new InvalidOperationException(ResponseMessagesFacturaErrors.OnlyCitaAllowed);
        }

        var consulta = await _repository.GetConsultaByCitaOrPetAsync(cita.Id, cita.MascotaId)
            ?? throw new KeyNotFoundException(ResponseMessagesWSMessageAPI.ConsultaNotFound);

        var factura = await _repository.GetFacturaByConsultaIdAsync(consulta.Id)
            ?? throw new KeyNotFoundException(ResponseMessagesWSMessageAPI.FacturaNotFound);

        var cliente = consulta.Mascota?.Cliente ?? cita.Mascota?.Cliente;
        if (cliente == null || string.IsNullOrWhiteSpace(cliente.Telefono))
        {
            throw new InvalidOperationException(ResponseMessagesWSMessageAPI.ClientErrorNumberPhone);
        } 
        
        var whatsappConfig = await _wsmRepository.GetWSMessageAPIDataAsync();
        string dbClientId = whatsappConfig?.ClientId ?? string.Empty;

        string petName = consulta.Mascota?.Nombre ?? cita.Mascota?.Nombre ?? ResponseMessagesWSMessageAPI.PetsDefault;
        string dbMessage = whatsappConfig?.Message ?? ResponseMessagesWSMessageAPI.MessageTempleteDefault;

        string finalMessage = GetPersonalizedMessage(dbMessage, petName);

        var clientFullName = $"{cliente.Nombre} {cliente.Apellido}".Trim();
        var payload = new WSMessageDTO
        {
            ClientId = dbClientId,
            Numero = cliente.Telefono,
            Cliente = clientFullName,
            NombreEmpresa = _apiSettings.SystemName!,
            Mensaje = finalMessage, 
            Url = !string.IsNullOrWhiteSpace(factura.UrlDocx)
                ? factura.UrlDocx
                : $"http://localhost:5168/facturas/{factura.NumeroFactura}.pdf"
        };

        bool dispatchResult = await _wsMessageService.EnviarComprobanteAsync(payload);

        return new InvoiceDispatchResponseDTO
        {
            InvoiceId = factura.Id,
            InvoiceNumber = factura.NumeroFactura,
            IsDispatched = dispatchResult,
            ClientName = clientFullName,
            DestinationPhone = cliente.Telefono
        };
    }
    private string GetPersonalizedMessage(string dbMessage, string petName)
    { 
        if (dbMessage.Contains("{0}"))
        {
            return string.Format(dbMessage, petName);
        } 
        if (dbMessage.Contains(ResponseMessagesWSMessageAPI.PetsDefault, StringComparison.OrdinalIgnoreCase))
        {
            return dbMessage.Replace(ResponseMessagesWSMessageAPI.PetsDefault, $"{ResponseMessagesWSMessageAPI.PetsDefault} {petName}", StringComparison.OrdinalIgnoreCase);
        } 
        return $"{dbMessage} ({petName})";
    }
}