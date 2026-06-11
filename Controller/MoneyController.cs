using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using DTOs;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Constants;
using vet_api_Net.Routes;
using vet_api_Net.HttpServices;

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
        try{
        if (money == null || string.IsNullOrWhiteSpace(money.MoneyName))
        {
            return BadRequest(ResponseMessagesMoneyTypes.ErrorUpdatingMoneyType);
        }

        var updatedMoneyType = await _moneyTypeService.UpdateMoneyTypeAsync(money);
        return Ok(updatedMoneyType);
        
    }catch(Exception ex)
        {
            return StatusCode(500, new { mensaje = ex.Message });
        }
    }

    [HttpGet(Endpoints.MoneyController.TasaDollarBcv)]
    public async Task<IActionResult> GetDirectoDeBcv()
    {
        decimal precio = await _scraper.ObtenerPrecioBcvAsync();

        if (precio <= 0)
        {
            return StatusCode(503, new
            {
                mensaje = ResponseMessagesMoneyTypes.GetTasaBcvError
            });
        }

        return Ok(new
        {
            moneda = "USD",
            precio_bs = precio,
            fecha_consulta = DateTime.Now,
            fuente = ResponseMessagesMoneyTypes.fuente
        });
    }
    [HttpGet(Endpoints.MoneyController.TasaDollarBcvToDb)]
    public async Task<IActionResult> GetTasaBcvAndSaveToDb()
    {
        var result = await _moneyTypeService.GetTasaDollarBcvAsync();

        if (result == null || result.BcvDollar <= 0)
        {
            return StatusCode(503, new
            {
                mensaje = ResponseMessagesMoneyTypes.GetTasaBcvError
            });
        }

        return Ok(result);

    }
}

