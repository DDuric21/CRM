using Backend_API.Services;
using Microsoft.AspNetCore.Mvc;
using Models.Authentication;
using Models.DTO;
using Models.Helpers;

namespace Backend_API.Controllers
{
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserService _userService;

        public AuthenticationController(
            IAuthenticationService authenticationService,
            IUserService userService)
        {
            _authenticationService = authenticationService;
            _userService = userService;
        }

        [HttpPost]
        [Route("/Login")]
        public async Task<IActionResult> LoginUser([FromBody] UserDTO userDTO)
        {
            if (userDTO.IsNullOrEmpty())
            {
                return BadRequest();
            }

            if (!await _authenticationService.ValidateLoginAsync(userDTO))
            {
                return Forbid();
            }

            var authenticationResult = await _authenticationService.GenerateJwtAuthenticationResultAsync(userDTO.UserName);

            if (!authenticationResult.IsAuthenticated)
            {
                return Problem("Token failed to generate!");
            }

            _authenticationService.SetRefreshTokenCookie(Response.Cookies, authenticationResult.RefreshToken);
            authenticationResult.RefreshToken = null;

            return Ok(authenticationResult);
        }

        [HttpPost]
        [Route("/Register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserDTO userDTO)
        {
            if (userDTO is null
                || string.IsNullOrWhiteSpace(userDTO.FirstName)
                || string.IsNullOrWhiteSpace(userDTO.LastName)
                || string.IsNullOrWhiteSpace(userDTO.Password))
            {
                return BadRequest();
            }

            try
            {
                var newUser = await _userService.CreateNewUserAsync(userDTO);

                if (newUser == null)
                {
                    return Problem("No user created!");
                }

                var newUserDTO = _userService.MapUserToDTO(newUser);

                return Ok(newUserDTO);
            }
            catch (Exception ex)
            {
                //add logging
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("/RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRQ tokenRequest)
        {
            var refreshToken = Request.Cookies[((AuthenticationService)_authenticationService).RefreshTokenCookieKey];
            var isValid = await _authenticationService.VerifyTokenRequestAsync(tokenRequest, refreshToken);

            if (!isValid)
            {
                _authenticationService.DeleteRefreshTokenCookie(Response.Cookies);
                return Unauthorized();
            }

            var authenticationResult = await _authenticationService.RefreshJwtAsync(tokenRequest.AccessToken, refreshToken);

            if (!authenticationResult.IsAuthenticated)
            {
                _authenticationService.DeleteRefreshTokenCookie(Response.Cookies);
                return Problem("Token failed refreshing!");
            }

            _authenticationService.SetRefreshTokenCookie(Response.Cookies, authenticationResult.RefreshToken);

            return Ok(authenticationResult.AccessToken);
        }
    }
}
