using System.Text.Json;
using System.Text.Json.Serialization;
using BackendLabs_2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using freecurrencyapi;

namespace BackendLabs_2.Controllers;

[ApiController]
[Route("record")]
public class RecordController(AppDbContext context, Freecurrencyapi fx) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult> GetRecord(int id)
    {
        var record = await context.Records.FirstOrDefaultAsync(r => r.Id == id);
        if (record == null)
        {
            return NotFound();
        }
        return Ok(record);
    }
    
    [HttpPost]
    public async Task<ActionResult> CreateRecord(
        [FromBody]CreateRecordRequest request)
    {
        var user = await context.Users
            .Include(user => user.DefaultCurrency)
            .FirstOrDefaultAsync(u => u.Id == request.UserId);
        if (user == null)
        {
            return BadRequest("User not found");
        }
        var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId);
        if (category == null)
        {
            return BadRequest("Category not found");
        }

        Currency currency = null!;
        if (request.CurrencySymbol == null)
        {
            currency = user.DefaultCurrency!;
        }
        else
        {
            currency = await context.Currencies
                .Where(c => c.Symbol == request.CurrencySymbol)
                .FirstOrDefaultAsync() ?? Currency.DefualtCurrency;
        }
        
        
        var record = Record.Create(user, category, request.TotalAmount, currency);
        
        await context.Records.AddAsync(record);
        await context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetRecord), new { id = record.Id }, record);
    }
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteRecord(int id)
    {
        var record = await context.Records.FirstOrDefaultAsync(r => r.Id == id);
        if (record == null)
        {
            return NotFound();
        }
        context.Records.Remove(record);
        await context.SaveChangesAsync();
        return NoContent();
    }
    
    [HttpGet]
    public async Task<ActionResult> GetRecords([FromQuery]int? userId, [FromQuery]int? categoryId)
    {
        if (userId == null && categoryId == null)
        {
            return BadRequest();
        }
        
        var filteredRecords = context.Records.AsQueryable();
        
        if (categoryId != null)
        {
            filteredRecords = filteredRecords.Where(p => p.CategoryId == categoryId);
        }
        
        if (userId != null)
        {
            filteredRecords = filteredRecords.Where(p => p.UserId == userId);
        }

        return await filteredRecords.AnyAsync() ?  Ok(await filteredRecords.ToListAsync()) : NotFound("No records found");
    }
    
    [HttpPut("{id:int}/currency")]
    public async Task<ActionResult> ChangeRecordCurrency(int id, [FromBody]ChangeRecordCurrencyRequest request)
    {
        var record = await context.Records
            .Include(record => record.Currency)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (record == null)
        {
            return NotFound();
        }
        var currency = await context.Currencies.FirstOrDefaultAsync(c => c.Symbol == request.CurrencySymbol);
        if (currency == null)
        {
            return BadRequest("Currency not found");
        }
        
        var coefJson = fx.Latest(record.Currency.Symbol, currency.Symbol);
        
        var coef = JsonSerializer.Deserialize<CurrencyResponse>(coefJson);

        var newTotalAmount = coef.Data.GetValueOrDefault(currency.Symbol)
             * record.TotalAmount;
        
        record.Currency = currency;
        record.TotalAmount = newTotalAmount;
        await context.SaveChangesAsync();
        return NoContent();
    }
    
}

public record ChangeRecordCurrencyRequest(string CurrencySymbol);


public record CreateRecordRequest(int UserId, int CategoryId, decimal TotalAmount, string? CurrencySymbol);

public class CurrencyResponse
{
    [JsonPropertyName("data")]
    public Dictionary<string, decimal> Data { get; set; }
}