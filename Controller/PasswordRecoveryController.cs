using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.DTOs;
using vet_api_Net.Constants;
using vet_api_Net.Routes;

namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PasswordRecoveryController : ControllerBase
{
    private readonly IPasswordRecoveryService _passwordRecoveryService;

    public PasswordRecoveryController(IPasswordRecoveryService passwordRecoveryService)
    {
        _passwordRecoveryService = passwordRecoveryService;
    }

    [HttpGet(Endpoints.PasswordRecovery.RequestCode)]
    public async Task<IActionResult> RequestRecoveryCode([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest(new { success = false, error = ResponseMessagesPasswordRecovery.RequiredEmail });
        }

        try
        {
            var result = await _passwordRecoveryService.RequestRecoveryCodeAsync(email);
            if (!result)
            {
                return NotFound(new { success = false, error = ResponseMessagesPasswordRecovery.ErrorRequestingCode });
            }

            return Ok(new { success = true, message = ResponseMessagesPasswordRecovery.CodeSentSuccess });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }

    [HttpPost(Endpoints.PasswordRecovery.VerifyCode)]
    public async Task<IActionResult> VerifyCodeAndSendPassword([FromBody] VerifyRecoveryCodeRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Code))
        {
            return BadRequest(new { success = false, error = ResponseMessagesPasswordRecovery.RequiredCode });
        }

        try
        {
            var result = await _passwordRecoveryService.VerifyCodeAndSendPasswordAsync(request.Code);
            if (!result)
            {
                return BadRequest(new { success = false, error = ResponseMessagesPasswordRecovery.InvalidCode });
            }

            return Ok(new { success = true, message = ResponseMessagesPasswordRecovery.CodeVerifiedSuccess });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }
}
