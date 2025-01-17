using Models.Authentication;
using Models.DTO;

namespace UI.Services
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> Login(string username, string password);

        Task<UserDTO> RegisterNewUserAsync(UserDTO userDTO);
    }
}
