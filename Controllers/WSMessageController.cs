using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DTOs;
using vet_api_Net.Services.WSMessage;
using vet_api_Net.Interfaces.Services;
using vet_api_Net.Constants;
using vet_api_Net.Routes;

namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WSMessageController : ControllerBase
{
    private readonly IInvoiceExternalService _invoiceExternalService;
    private readonly IWSMessage _wsMessage;

    public WSMessageController(IInvoiceExternalService invoiceExternalService, IWSMessage wsMessage)
    {
        _invoiceExternalService = invoiceExternalService;
        _wsMessage = wsMessage;
    }

    [HttpPost(Endpoints.WSMessageController.DispatchInvoiceByCita)]
    public async Task<IActionResult> DispatchInvoiceByCita([FromRoute] int citaId)
    {
        try
        {
            var result = await _invoiceExternalService.VerifyAndDispatchInvoiceByCitaAsync(citaId);

            if (result.IsDispatched)
            {
                return Ok(new 
                { 
                    success = true, 
                    message = $"{ResponseMessagesWSMessageController.Success} Cliente: {result.ClientName}",
                    data = result 
                });
            }
 
            return StatusCode(502, new 
            { 
                success = false, 
                message = $"{ResponseMessagesWSMessageController.PartialFailure} Cliente: {result.ClientName}",
                data = result 
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new 
            { 
                success = false, 
                message = ResponseMessagesWSMessageController.CriticalFailure, 
                details = ex.Message 
            });
        }
    }

    [HttpPost(Endpoints.WSMessageController.SessionInit)]
    public async Task<IActionResult> InitializeSession()
    {
        try
        {
            bool isSessionInitiated = await _wsMessage.IniciarSesionAsync();

            if (isSessionInitiated)
            {
                return Ok(new 
                { 
                    success = true, 
                    message = ResponseMessagesWSMessageController.SessionInitSuccess
                });
            }

            return StatusCode(502, new 
            { 
                success = false, 
                message = ResponseMessagesWSMessageController.SessionInitFailure
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new 
            { 
                success = false, 
                message = ResponseMessagesWSMessageController.SessionInitCriticalFailure, 
                details = ex.Message 
            });
        }
    }

    [HttpGet(Endpoints.WSMessageController.SessionStatus)]
    public async Task<IActionResult> GetSessionStatus()
    {
        try
        {
            var statusResult = await _wsMessage.ObtenerEstadoOSesionAsync();

            if (statusResult != null)
            {
                return Ok(new 
                { 
                    success = true, 
                    data = statusResult 
                });
            }

            return NotFound(new 
            { 
                success = false, 
                message = ResponseMessagesWSMessageController.SessionStatusNotFound
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new 
            { 
                success = false, 
                message = ResponseMessagesWSMessageController.SessionStatusCriticalFailure, 
                details = ex.Message 
            });
        }
    }
}