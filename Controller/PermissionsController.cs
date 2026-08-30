using System;
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
[Authorize]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionDefinitionService _permissionDefinitionService;

    public PermissionsController(IPermissionDefinitionService permissionDefinitionService)
    {
        _permissionDefinitionService = permissionDefinitionService;
    }

    [HttpGet(Endpoints.Permissions.Catalog)]
    public async Task<ActionResult<PermissionCatalogResponseDTO>> GetCatalog()
    {
        try
        {
            var result = await _permissionDefinitionService.GetCatalogAsync();
            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = ResponseMessagesPermissions.ErrorGettingCatalog });
        }
    }
}
