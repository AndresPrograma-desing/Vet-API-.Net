using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DTOs;
using vet_api_Net.Constants;
using vet_api_Net.Routes;
using vet_api_Net.Interfaze.Services;

namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkerConfigController : ControllerBase
{
    private readonly IWorkerConfigService _workerConfigService;

    public WorkerConfigController(IWorkerConfigService workerConfigService)
    {
        _workerConfigService = workerConfigService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var configs = await _workerConfigService.GetAllAsync();
        return Ok(configs);
    }

    [HttpGet(Endpoints.WorkerConfigController.GetByName)]
    public async Task<ActionResult<WorkerConfigDTO>> GetByName(string workerName)
    {
        try
        {
            var config = await _workerConfigService.GetByWorkerNameAsync(workerName);
            if (config == null) return NotFound(new { message = ResponseMessagesWorkerConfig.WorkerConfigNotFound });
            return Ok(config);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPut(Endpoints.WorkerConfigController.UpdateByName)]
    public async Task<ActionResult<WorkerConfigDTO>> UpdateByName(string workerName, [FromBody] UpdateWorkerConfigDTO data)
    {
        try
        {
            var updated = await _workerConfigService.UpdateAsync(workerName, data);
            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
