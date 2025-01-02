namespace BackendLabs_2.Services;

public interface IAuthenticationService
{
    Task<LoginResponseModel?> Login(LoginRequestModel request);
    Task<RegisterResponseModel?> Register(RegisterRequestModel request);
}