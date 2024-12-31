using BackendLabs_2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendLabs_2.Controllers;

[ApiController]
public class CurrencyController(AppDbContext context): ControllerBase
{
    
    [HttpGet("/currencies")]
    public async Task<IActionResult> GetCurrencies()
    {
        var currencies = await context.Currencies.ToListAsync();
        return currencies.Count != 0 ? Ok(currencies) : NotFound();
    }

    [HttpGet("/currency/{id}")]
    public async Task<IActionResult> GetCurrency(int id)
    {
        var currency = await context.Currencies.FirstOrDefaultAsync(c => c.Id == id);
        if (currency == null)
        {
            return NotFound();
        }
        return Ok(currency);
    }

    [HttpPost("/currency")]
    public async Task<IActionResult> CreateCurrency([FromBody] CreateCurrencyRequest request)
    {
        var isCurrencyExist = await context.Currencies.AnyAsync(c => c.Symbol == request.Symbol);
        if (isCurrencyExist)
        {
            return BadRequest("Currency already exists");
        }
        var currency = new Currency()
        {
            Symbol = request.Symbol,
            Name = request.Name
        };
        await context.Currencies.AddAsync(currency);
        await context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCurrency), new { id = currency.Id }, currency);
    }

    [HttpDelete("/currency/{id}")]
    public async Task<ActionResult> DeleteCurrency(int id)
    {
        var currency = await context.Currencies.FirstOrDefaultAsync(c => c.Id == id);
        if (currency == null)
        {
            return NotFound();
        }
        context.Currencies.Remove(currency);
        await context.SaveChangesAsync();
        return NoContent();
    }

    public record CreateCurrencyRequest(string Symbol, string Name);
}