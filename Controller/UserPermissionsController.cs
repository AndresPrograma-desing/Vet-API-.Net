using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Routes;

namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class UserPermissionsController : ControllerBase
{
    private readonly IUserPermissionService _userPermissionService;

    public UserPermissionsController(IUserPermissionService userPermissionService)
    {
        _userPermissionService = userPermissionService;
    }

    [HttpGet(Endpoints.UserPermissions.GetByUser)]
    public async Task<ActionResult<UserPermissionsResponseDTO>> GetByUser([FromRoute] int userId)
    {
        try
        {
            var permissions = await _userPermissionService.GetPermissionsAsync(userId);
            return Ok(new UserPermissionsResponseDTO { UserId = userId, Permissions = permissions });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseMessagesUserPermissions.ErrorGettingPermissions });
        }
    }

    [HttpPut(Endpoints.UserPermissions.Update)]
    public async Task<ActionResult<UserPermissionsResponseDTO>> Update([FromRoute] int userId, [FromBody] UpdateUserPermissionsDTO request)
    {
        if (request?.Permissions == null)
        {
            return BadRequest(new { error = ResponseMessagesUserPermissions.PermissionsRequired });
        }

        try
        {
            var permissions = await _userPermissionService.SetPermissionsAsync(userId, request.Permissions);
            return Ok(new UserPermissionsResponseDTO { UserId = userId, Permissions = permissions });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseMessagesUserPermissions.ErrorUpdatingPermissions });
        }
    }
}
