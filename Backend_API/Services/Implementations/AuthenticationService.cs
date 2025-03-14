using Backend_API.Data.Model;
using Backend_API.Data.Repositories;
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
        private readonly IConfiguration _configuration;
        private readonly int RefreshTokenLenght = 23;
        private readonly string RefreshTokenCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        public AuthenticationService(
            IConfiguration configuration,
            ICrmRepository repository,
            TokenValidationParameters tokenValidationParameters,
            CrmUserManager userManager,
            IUserService userService)
        {
            _tokenValidationParameters = tokenValidationParameters;
            _userManager = userManager;
            _repository = repository;
            _configuration = configuration;
            _userService = userService;
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

        public async Task<AuthenticationResult> GenerateJwtTokenAsync(string userName)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var secret = _configuration.GetSection("JwtConfiguration:Secret").Value;
            var key = Encoding.UTF8.GetBytes(secret);

            var userData = await _userService.GetUserDataAsync(userName);

            var claims = new List<Claim>
            {
                new Claim(CrmJwtClaimNames.Id, userData.User.Id),
                new Claim(JwtRegisteredClaimNames.Name, userData.User.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString()),
            };

            foreach (var role in userData.UserRoles)
            {
                claims.Add(new Claim(CrmJwtClaimNames.Role, role.Key.Name));
                claims.Add(new Claim(CrmJwtClaimNames.Role, "test"));

                foreach (var roleClaim in role.Value)
                {
                    claims.Add(new Claim(CrmJwtClaimNames.Permission, roleClaim.Value));
                }
            }

            var tokenDescripter = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(TimeSpan.Parse(_configuration.GetSection("JwtConfiguration:ExpiryTimeFrame").Value)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescripter);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            var existingRefreshToken = _repository.RefreshTokens
                .Where(x => x.UserId == userData.User.Id)
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
                UserId = userData.User.Id
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

            var utcExpiryDate = HelperMethods.ParseClaimExpiryToDatetime(claimExpiry);

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
