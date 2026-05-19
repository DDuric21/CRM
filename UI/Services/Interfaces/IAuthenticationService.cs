using Models.Authentication;
using Models.DTO;
using UI.Authentication;

namespace UI.Services
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> Login(string username, string password);

        Task<UserDTO> RegisterNewUserAsync(UserDTO userDTO);

        UserSession CreateUserSession();

        Task ReadAccessTokenFromLocalStorageAsync();

        Task SaveAccessTokenToLocalStorageAsync(string token);

        Task RemoveAccessTokenFromLocalStorageAsync();
    }
}
