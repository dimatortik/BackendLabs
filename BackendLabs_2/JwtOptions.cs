namespace BackendLabs_2;

public class JwtOptions
{
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public string? Key { get; set; }
    public string? TimeExpiration { get; set; }
}