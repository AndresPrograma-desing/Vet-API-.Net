using Microsoft.AspNetCore.Mvc;
using DTOs;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Constants;
using vet_api_Net.Routes;

namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/users")]
public class UserPetsController : ControllerBase
{
    private readonly IUserPetsService _userPetsService;

    public UserPetsController(IUserPetsService userPetsService)
    {
        _userPetsService = userPetsService;
    }

    [HttpGet(Endpoints.UsersPetsController.UserPets)]
    public async Task<IActionResult> UserPets(string nombre)
    {
        try
        {
            var mascotas = await _userPetsService.GetUserPetsAsync(nombre);
            return Ok(new UserPetsResponseDTO { Mascotas = mascotas });
        }
        catch (Exception)
        { 
            return StatusCode(500, new { error = ResponseMessagesUserPetsController.MascotaUserNotFound });
        }
    }
}
