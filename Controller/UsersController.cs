using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Models;
using vet_api_Net.Routes;

namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
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
    [Authorize]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO userDto)
    {
        try
        {
            var user = await _userService.CreateUserAsync(userDto);
            return Ok(user);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet(Endpoints.Users.Secretaries)]
    [Authorize(Roles = "admin,secretaria,doctor")]
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
        try
        {
            var user = await _userService.DeleteUserAsync(id);

            if (user == null)
            {
                _logger.LogInformation($"Usuario {id} no encontrado");
                return NotFound(new { message = $"{ResponseMessagesUsersController.UserIdNotFound} {id}" });
            }

            return Ok(new { message = ResponseMessagesUsers.DeletingUser(id) });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
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

    [HttpPost(Endpoints.Users.RequestPasswordReset)]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetDTO request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new { error = ResponseMessagesPasswordRecovery.RequiredEmail });

        try
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}/api/users/confirm-ticket";
            var result = await _userService.RequestPasswordResetAsync(request.Email, baseUrl);

            if (!result) return BadRequest(new { error = ResponseMessagesUsers.UserNotFound });

            return Ok(new { message = ResponseMessagesUsers.PasswordResetTicketCreated });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet(Endpoints.Users.ConfirmTicket)]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmTicket([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest(new { error = ResponseMessagesUsers.InvalidToken });

        try
        {
            var result = await _userService.ConfirmPasswordResetTicketAsync(token);
            if (!result) return BadRequest(new { error = ResponseMessagesUsers.PasswordResetTicketNotFoundOrExpired });

            var html = await _userService.GetTemplatePasswordResetPage(ResponseMessagesUsers.PasswordResetTicketAccepted);
            return Content(html ?? string.Empty, "text/html");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet(Endpoints.Users.ResetStatus)]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetResetStatus([FromRoute] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest(new { error = ResponseMessagesPasswordRecovery.RequiredEmail });

        try
        {
            var result = await _userService.GetResetStatusAsync(email);
            if (result == null) return BadRequest(new { error = ResponseMessagesUsers.UserNotFound });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet(Endpoints.Users.PendingResets)]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetPendingResets()
    {
        try
        {
            var result = await _userService.GetPendingPasswordResetsAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost(Endpoints.Users.AssignNewPassword)]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> AssignNewPassword([FromBody] AssignNewPasswordDTO request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.NewPassword))
            return BadRequest(new { error = ResponseMessagesUsers.EmailAndNewPasswordRequired });

        try
        {
            var result = await _userService.AssignNewPasswordAsync(request.Email, request.NewPassword);
            if (!result) return BadRequest(new { error = ResponseMessagesUsers.PasswordResetTicketNotFoundOrExpired });

            return Ok(new { message = ResponseMessagesUsers.PasswordAssignedSuccess });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPatch(Endpoints.Users.UpdateUserName)]
    public async Task<IActionResult> UpdateUserName([FromBody] ChangeNameUsersDTO request)
    {
        if (request == null || request.IdUser <= 0 || string.IsNullOrWhiteSpace(request.NewUserName))
            return BadRequest(new { error = ResponseMessagesUsers.NotUpdateNameUser });

        try
        {
            var result = await _userService.ChangeNameUsersAsync(request.IdUser, request.NewUserName, request.NewLastName, request.NewEmail, request.NewPhone);
            if (result == null ||
             string.IsNullOrEmpty(result.NewUserName) ||
             string.IsNullOrEmpty(result.NewLastName) ||
             string.IsNullOrEmpty(result.NewEmail) ||
             string.IsNullOrEmpty(result.NewPhone))
            {
                return BadRequest(new { error = ResponseMessagesUsers.NotUpdateNameUser });
            }

            return Ok(new { message = ResponseMessagesUsers.UpdateNameUser });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}