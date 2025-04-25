using Backend_API.Data.Models;
using Models.Authentication;
using Models.DTO;

namespace Backend_API.Services
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Validates weather a user has the permission to login
        /// </summary>
        /// <param name="userDTO">User that requested login</param>
        /// <returns>True if has permission, False if not</returns>
        Task<bool> ValidateLoginAsync(UserDTO userDTO);

        /// <summary>
        /// Verifies if the given token request is valid
        /// </summary>
        /// <param name="tokenRequest">Request containing data needed for access token to be refreshed</param>
        /// <param name="refreshToken">Token needed to refresh access token</param>
        /// <returns>Returns result of verification</returns>
        Task<bool> VerifyTokenRequestAsync(RefreshTokenRQ tokenRequest, string refreshToken);

        /// <summary>
        /// Generates a web token
        /// </summary>
        /// <param name="userName">UserName requesting login</param>
        /// <returns>Returns result of generating token</returns>
        Task<AuthenticationResult> GenerateJwtAuthenticationResultAsync(string userName);

        /// <summary>
        /// Finds user registered for given refresh token
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        /// <returns>Returns user associated with given refresh token</returns>
        Task<User> GetRefreshTokenUserAsync(string refreshToken);

        Task<AuthenticationResult> RefreshJwtAsync(string accessToken, string refreshToken);

        IEnumerable<string> ReadAccessTokenData(string accessToken, string claimName);

        void SetRefreshTokenCookie(IResponseCookies responseCookies, string refreshToken);

        public void DeleteRefreshTokenCookie(IResponseCookies responseCookies);
    }
}
