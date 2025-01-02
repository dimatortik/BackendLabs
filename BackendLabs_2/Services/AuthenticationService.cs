using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackendLabs_2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BackendLabs_2.Services;

public class AuthenticationService(
    AppDbContext context,
    PasswordHasher<User> passwordHasher,
    IOptions<JwtOptions> options)
    : IAuthenticationService
{

    public async Task<LoginResponseModel?> Login(LoginRequestModel request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Password))
            return null;
        
        var userAccount = await context.Users.FirstOrDefaultAsync(u => u.Name == request.Name);
        if (userAccount == null || string.IsNullOrWhiteSpace(userAccount.PasswordHash))
            return null;
        
        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(userAccount, userAccount.PasswordHash, request.Password);
        if (passwordVerificationResult != PasswordVerificationResult.Success)
            return null;
        
        var issuer = options.Value.Issuer;
        var audience = options.Value.Audience;
        var key = options.Value.Key;
        var tokenMin = Convert.ToInt32(options.Value.TimeExpiration);
        var tokenExpiryTimeStap = DateTime.UtcNow.AddMinutes(tokenMin);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Name, request.Name)
            }),
            Expires = tokenExpiryTimeStap,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(securityToken);

        return new LoginResponseModel
        (
            accessToken,
            request.Name,
            (int)tokenExpiryTimeStap.Subtract(DateTime.UtcNow).TotalSeconds
        );
    }
    
    public async Task<RegisterResponseModel?> Register(RegisterRequestModel request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return new RegisterResponseModel(null, null, "Username is required.");

        var existingUser = context.Users.FirstOrDefault(u => u.Name == request.Name);
        if (existingUser != null)
            return new RegisterResponseModel(null, null,"A user with this name already exists.");

        var currency = await GetCurrencyAsync(request.currencySymbol);
        if (currency == null)
            return new RegisterResponseModel(null, null, "Invalid currency ID or default currency not available.");
        
        var newUser = new User
        {
            Name = request.Name,
            DefaultCurrencyId = currency.Id,
            PasswordHash = passwordHasher.HashPassword(null, request.Password)
        };

        var user = await context.Users.AddAsync(newUser);
        await context.SaveChangesAsync();
        
        var issuer = options.Value.Issuer;
        var audience = options.Value.Audience;
        var key = options.Value.Key;
        var tokenMin = Convert.ToInt32(options.Value.TimeExpiration);
        var tokenExpiryTimeStap = DateTime.UtcNow.AddMinutes(tokenMin);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Name, request.Name)
            }),
            Expires = tokenExpiryTimeStap,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(securityToken);

        return new RegisterResponseModel(accessToken, user.Entity.Id, null); 
    }

    private async Task<Currency?> GetCurrencyAsync(string currencySymbol)
    {
        if (string.IsNullOrWhiteSpace(currencySymbol))
            return await context.Currencies.FirstOrDefaultAsync(c => c.Symbol == "USD");
        return await context.Currencies.FirstOrDefaultAsync(c => c.Symbol == currencySymbol);
    }
}

public record LoginRequestModel(string Name, string Password);

public record LoginResponseModel(string AccessToken, string Name, int ExpiresIn);

public record RegisterRequestModel(string Name, string Password, string currencySymbol);
public record RegisterResponseModel(string? AccessToken, int? Id,  string? ErrorMessage);

