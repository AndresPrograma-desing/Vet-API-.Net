using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;
using Microsoft.AspNetCore.Mvc;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Services.Clients;
using vet_api_Net.Models;
using vet_api_Net.Routes;

namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpGet(Endpoints.Client.GetById)]
    public async Task<ActionResult<Cliente>> GetById([FromRoute] int id)
    {
        try
        {
            var client = await _clientService.GetClientAsync(id);
            if (client == null) return NotFound();

            return Ok(client);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error Proviniente del el Controller de Clientes", details = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<ClientListResponseDTO>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? query = null)
    {
        try
        {
            var (items, totalCount) = await _clientService.GetAllClientsWithDetailsAsync(pageNumber, pageSize, query);
            return Ok(new ClientListResponseDTO { Items = items, TotalCount = totalCount });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error Proviniente del el Controller de Clientes", details = ex.Message });
        }
    }


    [HttpDelete]
    [Route(Endpoints.Clients.Delete)]
    public async Task<ActionResult<Cliente>> DeleteClientAsync([FromRoute] int id)
    {
        var client = await _clientService.DeleteClientAsync(id);

        if (client == null)
        {
            return NotFound(new { message = ResponseMessagesClient.NotClientDeleted });
        }

        return Ok(new { message = ResponseMessagesClient.ClientDeletedSuccess });
    }

    [HttpPut(Endpoints.Clients.Update)]
    public async Task<ActionResult<Cliente>> UpdateClientAsync([FromRoute] int id, [FromBody] UpdateClientDTO dto)
    {
        try
        {
            var result = await _clientService.UpdateClientAsync(id, dto);
            if (result == null) return NotFound(new { message = ResponseMessagesClient.ClientNotFound });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ResponseMessagesClient.ClientUpdatedError, details = ex.Message });
        }
    }
    [HttpGet("validate")]
    public async Task<ActionResult<ClientLookupResponseDTO>> GetByIdentificacionOrEmail([FromQuery] string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return BadRequest(new { message = "Debe proporcionar una cédula o correo electrónico." });
        }

        try
        {
            var client = await _clientService.GetByIdentificacionOrEmailAsync(identifier);

            if (client == null)
            {
                return NotFound(new { message = "No se encontró ningún cliente registrado con esa cédula o correo electrónico." });
            }

            return Ok(client);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al validar la cédula o email del cliente.", details = ex.Message });
        }
    }
}
