using Backend_API.Authentication;
using Backend_API.Data.DTO;
using Backend_API.Data.Model;
using Backend_API.HelperMethods;
using Backend_API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend_API.Controllers
{
    public class AuthenticationController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthenticationController(
            ILoginService loginService,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration)
        {
            _loginService = loginService;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("/Login")]
        public ActionResult LoginUser(UserDTO userDTO)
        {
            if (userDTO.IsNullOrEmpty())
            {
                return BadRequest();
            }

            if (!_loginService.ValidateLogin(userDTO))
            {
                return Forbid();
            }

            var identityUser = new IdentityUser
            {
                UserName = userDTO.UserName,
                Email = userDTO.UserEmail
            };

            var jwtToken = GenerateJwtToken(identityUser);

            return Ok(new AuthenticationResult
            {
                IsAuthenticated = true,
                Token = jwtToken
            });
        }

        [HttpPost]
        [Route("/Register")]
        public async Task<IActionResult> RegisterUser(UserDTO userDTO)
        {
            if (userDTO.IsNullOrEmpty())
            {
                return BadRequest();
            }

            var user = await _userManager.FindByEmailAsync(userDTO.UserEmail);
            var result = new AuthenticationResult();

            if (user != null)
            {
                result.IsAuthenticated = false;
                result.ErrorMessages = new List<string> { "User email already exists" };

                return BadRequest(result);
            }

            var newUser = new IdentityUser
            {
                Email = userDTO.UserEmail,
                UserName = userDTO.UserName
            };

            var isCreated = await _userManager.CreateAsync(newUser, userDTO.Password);

            if (!isCreated.Succeeded)
            {
                result.IsAuthenticated = false;
                result.ErrorMessages = new List<string> { "Server error" };

                return BadRequest(result);
            }

            var jwtToken = GenerateJwtToken(newUser);
            result.IsAuthenticated = true;
            result.Token = jwtToken;

            return Ok(result);
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var secret = _configuration.GetSection("JwtConfiguration:Secret").Value;
            var key = Encoding.UTF8.GetBytes(secret);

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
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescripter);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }
    }
}
