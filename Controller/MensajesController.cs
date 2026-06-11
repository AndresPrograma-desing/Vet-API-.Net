using Microsoft.AspNetCore.Mvc;
using vet_api_Net.Interfaze.Services;
using DTOs;
using vet_api_Net.Routes;

namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MensajesController : ControllerBase
{
    private readonly IMessagingService _messagingService;

    public MensajesController(IMessagingService messagingService)
    {
        _messagingService = messagingService;
    }

    [HttpPost]
    public async Task<IActionResult> Send([FromBody] CreateMensajeDTO dto)
    {
        try
        {
            var msg = await _messagingService.SendMessageAsync(dto);
            return Ok(msg);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet(Endpoints.MassagesController.Conversation)]
    public async Task<IActionResult> Conversation(int userId, int otherId)
    {
        try
        {
            var msgs = await _messagingService.GetConversationAsync(userId, otherId);
            return Ok(msgs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet(Endpoints.MassagesController.UserMessages)]
    public async Task<IActionResult> GetUserMessages(int userId)
    {
        try
        {
            var msgs = await _messagingService.GetUserMessagesAsync(userId);
            return Ok(msgs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPatch(Endpoints.MassagesController.MarkRead)]
    public async Task<IActionResult> MarkRead(int id)
    {
        try
        {
            await _messagingService.MarkAsReadAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
