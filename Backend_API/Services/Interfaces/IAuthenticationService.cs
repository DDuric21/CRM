using Backend_API.Data.Model;
using Models.Authentication;
using Models.DTO;

namespace Backend_API.Services
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Validates wether a user has the permission to login
        /// </summary>
        /// <param name="userDTO">User that requested login</param>
        /// <returns>True if has perrmision, False if not</returns>
        bool ValidateLogin(UserDTO userDTO);

        /// <summary>
        /// Verifies if the given token request is valid
        /// </summary>
        /// <param name="tokenRequest">Request containing acces and refresh token</param>
        /// <returns>Returns result of verification</returns>
        AuthenticationResult VerifyTokenRequest(TokenRequest tokenRequest);

        /// <summary>
        /// Generates a web token
        /// </summary>
        /// <param name="user">User requesting login</param>
        /// <returns>Returns result of generating token</returns>
        AuthenticationResult GenerateJwtToken(User user);

        /// <summary>
        /// Finds user registrated for given refresh token
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        /// <returns>Returns user assosiated with given refresh token</returns>
        Task<User> GetRefreshTokenUser(string refreshToken);
    }
}
