using BackendLabs_2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendLabs_2.Controllers;
[ApiController]
public class UserController(AppDbContext context) : ControllerBase
{
    [HttpGet("/users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await context.Users.ToListAsync();
        return users.Count != 0 ? Ok(users) : NotFound();
    }
    
    
    [HttpGet("/user/{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }
    
    [HttpPost("/user")]
    public async Task<IActionResult> CreateUser([FromBody]CreateUserRequest request)
    {
        var currency = await context.Currencies.FirstOrDefaultAsync(c => c.Symbol == request.DefaultCurrencySymbol);
        if (currency == null)
        {
            return BadRequest("Currency not found");
        }

        try
        {
            var user = Models.User.Create(request.Name, currency);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        } 
        
    }
    
    [HttpDelete("/user/{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return NoContent();
    }
    
    [HttpPut("/user/{id}/currency/")]
    public async Task<ActionResult> ChangeUserCurrency(int id, [FromBody]ChangeUserCurrencyRequest request)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        var currency = await context.Currencies.FirstOrDefaultAsync(c => c.Symbol == request.CurrencySymbol);
        if (currency == null)
        {
            return BadRequest("Currency not found");
        }
        user.DefaultCurrency = currency;
        await context.SaveChangesAsync();
        return NoContent();
    }
}

public record ChangeUserCurrencyRequest(string CurrencySymbol);


public record CreateUserRequest(string Name, string DefaultCurrencySymbol);