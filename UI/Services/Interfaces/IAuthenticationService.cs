using Models.Authentication;

namespace UI.Services
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> Login(string username, string password);
    }
}
