using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using vet_api_Net.Constants;

namespace vet_api_Net.Controller;
[ApiController]

[Route("api/[controller]")]
[AllowAnonymous]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult GetHealth()
    {
    
        return Ok(new { status = ResponseMessagesHealthController.Message, timestamp = DateTime.UtcNow });

    }
}

