using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DTOs;
using vet_api_Net.Routes;
using vet_api_Net.Constants;

using vet_api_Net.Interfaze.Services.Clients;

namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/ClientPets")]
public class ClientPetController : ControllerBase
{
    private readonly IClientPetService _clientPetService;

    public ClientPetController(IClientPetService clientPetService)
    {
        _clientPetService = clientPetService;
    }

    [HttpPost(Endpoints.ClientPets.CreateWithPet)]
    public async Task<ActionResult<CreateClientWithPetResponseDTO>> Create([FromBody] CreateClientWithPetDTO dto)
    {
        try
        {
            var result = await _clientPetService.CreateClientWithPetAsync(dto);
            return StatusCode(201, result);
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = ResponseMessagesClientPetController.InternalErrorCP });
        }
    }

    [HttpPost(Endpoints.ClientPets.CreatePetForExistingClient)]
public async Task<IActionResult> CreatePetForExistingClient([FromBody] CreatePetForExistingClientDTO dto)
{
    try
    {
        var response = await _clientPetService.CreatePetForExistingClientAsync(dto);
        return StatusCode(201, response);
    }
    catch (KeyNotFoundException ex)
    {
        return NotFound(new { message = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(new { message = ex.Message });
    }
    catch (Exception)
    {
        return StatusCode(500, new { message = ResponseMessagesClientPetController.InternalErrorCP });
    }
}

    [HttpPost(Endpoints.ClientPets.UploadClientImage)]
    public async Task<IActionResult> UploadClientImage([FromRoute] int id, IFormFile image)
    {
        try
        {
            var imageUrl = await _clientPetService.UploadClientImageAsync(id, image);
            return Ok(new { image_url = imageUrl });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseMessagesClientPetController.ErrorUploadingImage });
        }
    }

    [HttpPost(Endpoints.ClientPets.UploadPetImage)]
    public async Task<IActionResult> UploadPetImage([FromRoute] int id, IFormFile image)
    {
        try
        {
            var imageUrl = await _clientPetService.UploadPetImageAsync(id, image);
            return Ok(new { image_url = imageUrl });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseMessagesClientPetController.ErrorUploadingImage });
        }
    }

    [HttpGet(Endpoints.ClientPets.GetClientsWithPetsLookup)]
    public async Task<IActionResult> GetClientsWithPetsLookup([FromQuery] string? query)
    {
        try
        {
            var searchTerm = query ?? string.Empty;
            var result = await _clientPetService.GetClientsWithPetsLookupAsync(searchTerm);
            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = ResponseMessagesClientPetController.InternalErrorCP });
        }
    }
}
