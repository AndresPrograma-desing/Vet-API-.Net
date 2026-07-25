using System;
using System.Threading.Tasks;
using DTOs;
using Microsoft.AspNetCore.Mvc;
using vet_api_Net.Constants;
using vet_api_Net.Interface.Services;
using vet_api_Net.Routes;

//Describe: Controlador HTTP para interactuar directamente con la inteligencia artificial de Groq para consultas y chats libres.
namespace vet_api_Net.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NowGrodController : ControllerBase
{
    private readonly INowGrodService _groqService;

    public NowGrodController(INowGrodService groqService)
    {
        _groqService = groqService;
    }

    [HttpPost(Endpoints.NowGrod.Consultar)]
    public async Task<ActionResult<GroqChatResponseDTO>> Consultar([FromBody] GroqChatRequestDTO dto)
    {
        try
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Pregunta))
            {
                return BadRequest(new { error = ResponseMessagesGroq.InvalidQuestion });
            }

            if (string.IsNullOrEmpty(dto.ContactoId))
            {
                dto.ContactoId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("id")?.Value
                              ?? "anonymous_session";
            }

            var result = await _groqService.EnviarConsultaAsync(dto);
            return Ok(result);
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
}
