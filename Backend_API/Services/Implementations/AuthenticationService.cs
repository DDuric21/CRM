using Models.Authentication;
using Models.DTO;
using Backend_API.Data.Model;
using Backend_API.Data.Repositories;
using Models.HelperMethods;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend_API.Services
{
    public sealed class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly ICrmRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly int RefreshTokenLenght = 23;
        private readonly string RefreshTokenCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        public AuthenticationService(
            IConfiguration configuration,
            ICrmRepository repository,
            TokenValidationParameters tokenValidationParameters,
            UserManager<IdentityUser> userManager)
        {
            _tokenValidationParameters = tokenValidationParameters;
            _userManager = userManager;
            _repository = repository;
            _configuration = configuration;
        }

        public bool ValidateLogin(UserDTO userDTO)
        {
            var user = _userManager.FindByEmailAsync(userDTO.UserEmail).Result;

            if (user.IsNullOrEmpty())
            {
                return false;
            }
            var isPasswordCorrect = _userManager.CheckPasswordAsync(user, userDTO.Password).Result;
            if (!isPasswordCorrect)
            {
                return false;
            }

            return true;
        }

        public AuthenticationResult VerifyTokenRequest(TokenRequest tokenRequest)
        {
            var authenticationResult = VerifyAccessToken(tokenRequest.Token);

            if (authenticationResult.ErrorMessages.Any(x => x.Equals("Token expired"))) //možda loše rješenje
            {
                var refreshTokenValidationResult = VerifyRefreshToken(tokenRequest.RefreshToken);
                authenticationResult.IsAuthenticated = refreshTokenValidationResult.IsAuthenticated;
            }

            return authenticationResult;
        }

        public AuthenticationResult GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var secret = _configuration.GetSection("JwtConfiguration:Secret").Value;
            var key = Encoding.UTF8.GetBytes(secret);

            user = _userManager.FindByEmailAsync(user.Email).Result;

            var claims = new[]
            {
                new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
            };

            var tokenDescripter = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(TimeSpan.Parse(_configuration.GetSection("JwtConfiguration:ExpiryTimeFrame").Value)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescripter);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            var existingRefreshToken = _repository.RefreshTokens
                .Where(x => x.UserId == user.Id)
                .Select(x => new
                {
                    x.ExpiryDate,
                    x.Token
                })
                .OrderByDescending(x => x.ExpiryDate)
                .FirstOrDefault();

            var refreshToken = new RefreshToken
            {
                Token = existingRefreshToken?.Token,
                TokenId = token.Id,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                IsRevoked = false,
                IsUsed = false,
                UserId = user.Id
            };

            if (existingRefreshToken.IsNullOrEmpty()
                || existingRefreshToken?.ExpiryDate < DateTime.UtcNow)
            {
                refreshToken.Token = GenerateRefreshToken();

                _repository.RefreshTokens.Insert(refreshToken);
            }

            return new AuthenticationResult
            {
                Token = jwtToken,
                RefreshToken = refreshToken.Token,
                IsAuthenticated = true
            };
        }

        public async Task<IdentityUser> GetRefreshTokenUser(string refreshToken)
        {
            var userId = _repository.RefreshTokens
                .Where(x => x.Token == refreshToken)
                .Select(x => x.UserId)
                .FirstOrDefault();

            if (userId == null)
            {
                return new IdentityUser();
            }

            var user = await _userManager.FindByIdAsync(userId);
            return user;
        }

        private string GenerateRefreshToken()
        {
            var random = new Random();
            return new string(Enumerable.Repeat(RefreshTokenCharacters, RefreshTokenLenght)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private AuthenticationResult VerifyAccessToken(string accessToken)
        {
            var authenticationResult = new AuthenticationResult();

            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var tokenInVerification = jwtTokenHandler.ValidateToken(accessToken, _tokenValidationParameters, out var validatedToken);

            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                if (!result)
                {
                    authenticationResult.ErrorMessages.Add("Token is not valid.");

                    return authenticationResult;
                }
            }
            var claimExpiry = tokenInVerification.Claims
                .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)
                .Value;

            var utcExpiryDate = Helper.ParseClaimExpiryToDatetime(claimExpiry);

            if (utcExpiryDate < DateTime.UtcNow)
            {
                authenticationResult.ErrorMessages.Add("Token expired");
            }

            authenticationResult.IsAuthenticated = authenticationResult.ErrorMessages.Count == 0;

            return authenticationResult;
        }

        private AuthenticationResult VerifyRefreshToken(string refreshToken)
        {
            AuthenticationResult authenticationResult = new AuthenticationResult();

            var storedRefreshToken = _repository.RefreshTokens
                .Where(x => x.Token == refreshToken)
                .FirstOrDefault();

            if (storedRefreshToken == null)
            {
                authenticationResult.ErrorMessages.Add("Refresh token doesn't exist");
                return authenticationResult;
            }

            if (storedRefreshToken.IsUsed
                || storedRefreshToken.IsRevoked)
            {
                authenticationResult.ErrorMessages.Add("RefreshToken is already used or has been revoked");
                return authenticationResult;
            }

            if (storedRefreshToken.ExpiryDate < DateTime.UtcNow)
            {
                authenticationResult.ErrorMessages.Add("RefreshToken is expired.");
                return authenticationResult;
            }

            authenticationResult.IsAuthenticated = authenticationResult.ErrorMessages.Count == 0;

            return authenticationResult;
        }

    }
}
