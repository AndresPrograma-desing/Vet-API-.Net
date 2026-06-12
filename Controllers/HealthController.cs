using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using vet_api_Net.Constants;

namespace vet_api_Net.Controllers;
[ApiController]

[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult GetHealth()
    {
    
        return Ok(new { status = ResponseMessagesHealthController.Message, timestamp = DateTime.UtcNow });

    }
}

