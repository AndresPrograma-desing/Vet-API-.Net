using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DTOs;
using vet_api_Net.Models;
using vet_api_Net.Interfaces.Services;
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
            return StatusCode(500, new { message = "Error retrieving client information.", details = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<ClientListWithMascotasDTO>>> GetAll()
    {
        try
        {
            var result = await _clientService.GetAllClientsWithDetailsAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving clients catalog with details.", details = ex.Message });
        }
    }
}
