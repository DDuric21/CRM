using Backend_API.Authentication;
using Backend_API.Data.DTO;
using Backend_API.HelperMethods;
using Backend_API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend_API.Controllers
{
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthenticationController(
            IAuthenticationService authenticationService,
            UserManager<IdentityUser> userManager)
        {
            _authenticationService = authenticationService;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("/Login")]
        public ActionResult LoginUser(UserDTO userDTO)
        {
            if (userDTO.IsNullOrEmpty())
            {
                return BadRequest();
            }

            if (!_authenticationService.ValidateLogin(userDTO))
            {
                return Forbid();
            }

            var identityUser = new IdentityUser
            {
                UserName = userDTO.UserName,
                Email = userDTO.UserEmail
            };

            var jwtToken = _authenticationService.GenerateJwtToken(identityUser);

            return Ok(new AuthenticationResult
            {
                IsAuthenticated = true,
                Token = jwtToken.Token,
                RefreshToken = jwtToken.RefreshToken
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

            result = _authenticationService.GenerateJwtToken(newUser);

            return Ok(result);
        }

        [HttpPost]
        [Route("/RefreshToken")]
        public async Task<IActionResult> RefreshToken(TokenRequest tokenRequest)
        {
            var authenticationResult = _authenticationService.VerifyTokenRequest(tokenRequest);

            if (!authenticationResult.IsAuthenticated)
            {
                return SignOut();
            }

            var refreshTokenUser = _authenticationService.GetRefreshTokenUser(tokenRequest.RefreshToken);

            var jwtToken = _authenticationService.GenerateJwtToken(refreshTokenUser.Result);

            authenticationResult.IsAuthenticated = true;
            authenticationResult.Token = jwtToken.Token;
            authenticationResult.RefreshToken = jwtToken.RefreshToken;

            return Ok(authenticationResult);
        }
    }
}
