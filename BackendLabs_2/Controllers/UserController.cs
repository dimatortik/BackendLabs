using BackendLabs_2.Models;
using BackendLabs_2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendLabs_2.Controllers;
[ApiController]
[Authorize]
public class UserController(AppDbContext context, IAuthenticationService auth) : ControllerBase
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
    
    [HttpPost("/register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterUser([FromBody]RegisterRequestModel request)
    {
        var result = await auth.Register(request);

        if (result?.ErrorMessage is not null)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result);
    }
    [HttpPost("/login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginUser([FromBody]LoginRequestModel request)
    {
        var result = await auth.Login(request);

        if (result is null)
        {
            return BadRequest("Invalid username or password");
        }
        return Ok(result);
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