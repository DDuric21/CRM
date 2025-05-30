using Backend_API.Data.DataClasses;
using Backend_API.Data.Models;
using Backend_API.Data.Repositories;
using Backend_API.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models.Authentication;
using Models.DTO;
using Models.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend_API.Services
{
    public sealed class AuthenticationService : IAuthenticationService
    {
        private readonly CrmUserManager _userManager;
        private readonly IUserService _userService;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly ICrmRepository _repository;
        private readonly JwtConfiguration _configuration;
        private readonly int RefreshTokenLenght = 23;
        private readonly string RefreshTokenCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        private readonly JwtSecurityTokenHandler _jwtTokenHandler;
        public readonly string RefreshTokenCookieKey = "refreshToken";

        public AuthenticationService(
            IOptions<JwtConfiguration> configuration,
            ICrmRepository repository,
            TokenValidationParameters tokenValidationParameters,
            CrmUserManager userManager,
            IUserService userService)
        {
            _tokenValidationParameters = tokenValidationParameters;
            _userManager = userManager;
            _repository = repository;
            _configuration = configuration.Value;
            _userService = userService;

            _jwtTokenHandler = new JwtSecurityTokenHandler();
        }

        public async Task<bool> ValidateLoginAsync(UserDTO userDTO)
        {
            var user = await _userManager.FindByNameAsync(userDTO.UserName);
            if (user.IsNullOrEmpty())
            {
                return false;
            }

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, userDTO.Password);
            if (!isPasswordCorrect)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> VerifyTokenRequestAsync(RefreshTokenRQ tokenRequest, string refreshToken)
        {
            bool isAccessTokenValid = ValidateAccessToken(tokenRequest.AccessToken);

            bool isRefreshTokenValid = await ValidateRefreshTokenAsync(refreshToken);

            var isValid = isAccessTokenValid && isRefreshTokenValid;

            return isValid;
        }

        public async Task<AuthenticationResult> RefreshJwtAsync(string accessToken, string refreshToken)
        {
            var userName = ReadAccessTokenData(accessToken, JwtRegisteredClaimNames.Name)
                .FirstOrDefault();

            if (userName is null)
            {
                return null;
            }

            var result = new AuthenticationResult();
            try
            {
                await MarkRefreshTokenAsUsed(refreshToken);

                result = await GenerateJwtAuthenticationResultAsync(userName);
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
                result.IsAuthenticated = false;
            }

            return result;
        }

        public async Task<AuthenticationResult> GenerateJwtAuthenticationResultAsync(string userName)
        {
            var result = new AuthenticationResult();

            if (string.IsNullOrWhiteSpace(userName))
            {
                return result;
            }

            try
            {
                var userData = await _userService.GetUserDataByNameAsync(userName);

                var token = GenerateSecurityToken(userData);
                var jwt = _jwtTokenHandler.WriteToken(token);

                var refreshToken = await HandleRefreshToken(userData, token);

                result = new AuthenticationResult
                {
                    AccessToken = jwt,
                    RefreshToken = refreshToken.Token,
                    IsAuthenticated = true
                };
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
            }

            return result;
        }

        private async Task MarkRefreshTokenAsUsed(string refreshToken)
        {
            var token = await _repository.RefreshTokens
                .Where(x => x.Token == refreshToken)
                .FirstAsync();

            token.IsUsed = true;

            await _repository.RefreshTokens.PartialUpdateAsync(token, x => x.IsUsed);
        }

        private async Task<RefreshToken> HandleRefreshToken(UserData userData, SecurityToken token)
        {
            var refreshToken = await GetValidUserRefreshTokenAsync(userData);

            if (refreshToken is null)
            {
                refreshToken = await GenerateRefreshTokenAsync(userData, token);
            }
            else
            {
                if (refreshToken.AccessTokenId != token.Id)
                {
                    refreshToken.AccessTokenId = token.Id;
                    await _repository.RefreshTokens.PartialUpdateAsync(refreshToken, x => x.AccessTokenId);
                }
            }

            return refreshToken;
        }

        private SecurityToken GenerateSecurityToken(UserData userData)
        {
            var key = Encoding.UTF8.GetBytes(_configuration.Secret);

            var claims = GenerateJwtClaims(userData);

            var tokenDescripter = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _configuration.Issuer,
                Audience = _configuration.Audience,
                Expires = DateTime.UtcNow.Add(_configuration.ExpiryTimeFrame),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = _jwtTokenHandler.CreateToken(tokenDescripter);
            
            return token;
        }

        public IEnumerable<string> ReadAccessTokenData(string accessToken, string claimName)
        {
            var token = _jwtTokenHandler.ReadJwtToken(accessToken);

            var claimValues = token.Claims
                .Where(x => x.Type == claimName)
                .Select(x => x.Value);

            return claimValues;
        }

        public async Task<User> GetRefreshTokenUserAsync(string refreshToken)
        {
            var userId = _repository.RefreshTokens
                .Where(x => x.Token == refreshToken)
                .Select(x => x.UserId)
                .FirstOrDefault();

            if (userId == null)
            {
                return new User();
            }

            var user = await _userManager.FindByIdAsync(userId);
            return user;
        }

        public void SetRefreshTokenCookie(IResponseCookies responseCookies, string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7),
                Path = "/",
                IsEssential = true,
            };

            responseCookies.Append(RefreshTokenCookieKey, refreshToken, cookieOptions);
        }

        public void DeleteRefreshTokenCookie(IResponseCookies responseCookies)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(-1)
            };

            responseCookies.Append("refreshToken", string.Empty, cookieOptions);
        }

        private async Task<RefreshToken> GenerateRefreshTokenAsync(UserData userData, SecurityToken accessToken)
        {
            var newRefreshTokenValue = GenerateNewRefreshToken();

            var newRefreshToken = new RefreshToken
            {
                Token = newRefreshTokenValue,
                AccessTokenId = accessToken.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(6),
                UserId = userData.User.Id
            };

            await _repository.RefreshTokens.InsertAsync(newRefreshToken);

            return newRefreshToken;
        }

        private async Task<RefreshToken> GetValidUserRefreshTokenAsync(UserData userData)
        {
             var validRefrrefreshToken = await _repository.RefreshTokens
                .Where(x => x.UserId == userData.User.Id
                    && !x.IsRevoked
                    && !x.IsUsed
                    && x.ExpiryDate > DateTime.UtcNow)
                .FirstOrDefaultAsync();

            return validRefrrefreshToken;
        }

        private string GenerateNewRefreshToken()
        {
            var random = new Random();
            return new string(Enumerable.Repeat(RefreshTokenCharacters, RefreshTokenLenght)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private bool ValidateAccessToken(string accessToken)
        {
            try
            {
                _jwtTokenHandler.ValidateToken(accessToken, _tokenValidationParameters, out var validatedToken);
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
                return false;
            }

            return true;
        }

        private List<Claim> GenerateJwtClaims(UserData userData)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, userData.User.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString()),
            };

            foreach (var role in userData.UserRoles)
            {
                claims.Add(new Claim(CrmJwtClaimNames.Role, role.Key.Name));

                foreach (var roleClaim in role.Value)
                {
                    claims.Add(new Claim(CrmJwtClaimNames.Permission, roleClaim.Value));
                }
            }

            return claims;
        }

        private async Task<bool> ValidateRefreshTokenAsync(string refreshToken)
        {
            var storedRefreshToken = await _repository.RefreshTokens
                .Where(x => x.Token == refreshToken)
                .FirstOrDefaultAsync();

            if (storedRefreshToken is null)
            {
                // add logging
                return false;
            }

            if (storedRefreshToken.IsUsed
                || storedRefreshToken.IsRevoked)
            {
                // add logging
                return false;
            }

            if (storedRefreshToken.ExpiryDate < DateTime.UtcNow)
            {
                // add logging
                return false;
            }

            return true;
        }
    }
}
