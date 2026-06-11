using vet_api_Net.Interfaze.Services;
using Microsoft.AspNetCore.Mvc;
using vet_api_Net.Models;
using DTOs;
using vet_api_Net.Constants;
using vet_api_Net.Routes;

namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]

public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers(){
        try
        {
            var users = await _userService.GetAllUserAsync();
            users.ForEach(u => u.Password = "[PROTECTED]");
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost(Endpoints.Users.Create)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO userDto)
    {
        try
        {
            var user = await _userService.CreateUserAsync(userDto);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet(Endpoints.Users.Secretaries)]
    public async Task<IActionResult> GetSecretarias()
    {
        try
        {
            var secretarias = await _userService.GetSecretariasAsync();
            secretarias.ForEach(u => u.Password = "[PROTECTED]");
            return Ok(secretarias);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

  [HttpPatch(Endpoints.Users.UserDisabled)]
    public async Task<IActionResult> Disable(int id)
    {
        var user = await _userService.DisableUserAsync(id);
        
        if (user == null)
        {
            return NotFound(new { message = $"{ResponseMessagesUsersController.UserIdNotFound} {id}" });
        }

        return Ok(user);
    }

    [HttpPatch(Endpoints.Users.UserEnabled)]
    public async Task<IActionResult> Enable(int id)
    {
        var user = await _userService.EnableUserAsync(id);
        
        if (user == null)
        {
            return NotFound(new { message = $"{ResponseMessagesUsersController.UserIdNotFound} {id}" });
        }

        return Ok(user);
    }

    [HttpDelete(Endpoints.Users.Delete)]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userService.DeleteUserAsync(id);
        
        if (user == null)
        {
            return NotFound(new { message = $"{ResponseMessagesUsersController.UserIdNotFound} {id}" });
        }

        return Ok(new { message = ResponseMessagesUsers.DeletingUser(id) });
}

[HttpGet(Endpoints.Users.Status)]
public async Task<IActionResult> GetStatus(int id)
{
    var status = await _userService.UserStatusAsync(id);

    if (status == null)
    {
        return NotFound(new { message = ResponseMessagesUsers.UserNotFound });
    }

    return Ok(new { id, status });
}
}