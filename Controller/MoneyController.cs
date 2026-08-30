using System.Collections.Generic;
using System.Linq;
using DTOs;
using Microsoft.AspNetCore.Mvc;
using vet_api_Net.Constants;
using vet_api_Net.HttpServices;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Routes;

namespace vet_api_Net.Controller;

[ApiController]
[Route("api/[controller]")]
public class MoneyController : ControllerBase
{
    private readonly IMoneyTypeService _moneyTypeService;
    private readonly IBcvScraper _scraper;


    public MoneyController(IMoneyTypeService moneyTypeService, IBcvScraper scraper)
    {
        _moneyTypeService = moneyTypeService;
        _scraper = scraper;
    }

    [HttpGet]
    public async Task<ActionResult<MoneyTypesDTO?>> GetMoneyType()
    {
        try
        {
            var moneyType = await _moneyTypeService.GetMoneyTypeAsync();
            if (moneyType == null) return NotFound(ResponseMessagesMoneyTypes.MoneyNotFound);
            return Ok(moneyType);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPut(Endpoints.MoneyController.Update)]
    public async Task<ActionResult<MoneyTypesDTO>> UpdateMoneyType(MoneyTypesDTO money)
    {
        try
        {
            if (money == null || string.IsNullOrWhiteSpace(money.MoneyName))
            {
                return BadRequest(ResponseMessagesMoneyTypes.ErrorUpdatingMoneyType);
            }

            var updatedMoneyType = await _moneyTypeService.UpdateMoneyTypeAsync(money);
            return Ok(updatedMoneyType);

        }
        catch (Exception)
        {
            return StatusCode(500, new { mensaje = ResponseErrors.InternalServerError });
        }
    }

    [HttpGet(Endpoints.MoneyController.TasaDollarBcv)]
    public async Task<IActionResult> GetDirectoDeBcv()
    {
        decimal? precio = await _scraper.ObtenerPrecioBcvAsync();
 
        if (!precio.HasValue || precio.Value <= 0)
        {
            return StatusCode(503, new
            {
                mensaje = ResponseMessagesMoneyTypes.GetTasaBcvError
            });
        }
 
        return Ok(new
        {
            moneda = "USD",
            precio_bs = precio.Value,
            fecha_consulta = DateTime.Now,
            fuente = ResponseMessagesMoneyTypes.fuente
        });
    }
    [HttpGet(Endpoints.MoneyController.TasaDollarBcvToDb)]
    public async Task<IActionResult> GetTasaBcvAndSaveToDb()
    {
        var result = await _moneyTypeService.GetTasaDollarBcvAsync();

        if (result == null || result.BcvDollar == ResponseMessagesMoneyTypes.BcvFallen)
        {
            return StatusCode(503, result);
        }

        return Ok(result);
    }

    [HttpPut(Endpoints.MoneyController.UpdateBcvManual)]
    public async Task<IActionResult> UpdateBcvManual([FromBody] UpdateBcvPriceRequest request)
    {
        try
        {
            if (request == null || request.Price <= 0)
            {
                return BadRequest(new { mensaje = ResponseMessagesMoneyTypes.InvalidPrice });
            }

            var result = await _moneyTypeService.UpdateBcvDollarPriceAsync(request.Price);
            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensaje = ResponseErrors.InternalServerError });
        }
    }

    [HttpPost(Endpoints.MoneyController.ScrapeAndSaveBcv)]
    public async Task<IActionResult> ScrapeAndSaveBcv()
    {
        try
        {
            decimal? precio = await _scraper.ObtenerPrecioBcvAsync();
 
            if (!precio.HasValue || precio.Value <= 0)
            {
                return StatusCode(503, new
                {
                    mensaje = ResponseMessagesMoneyTypes.GetTasaBcvError
                });
            }
            var result = await _moneyTypeService.UpdateBcvDollarPriceAsync(precio.Value);
            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensaje = ResponseErrors.InternalServerError });
        }
    }
}

