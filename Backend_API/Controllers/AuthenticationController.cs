using Backend_API.Data.Model;
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

            var jwtToken = await _authenticationService.GenerateJwtTokenAsync(userDTO.UserName);

            return Ok(new AuthenticationResult
            {
                IsAuthenticated = true,
                Token = jwtToken.Token,
                RefreshToken = jwtToken.RefreshToken
            });
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
        public async Task<IActionResult> RefreshToken(TokenRequest tokenRequest)
        {
            var authenticationResult = _authenticationService.VerifyTokenRequest(tokenRequest);

            if (!authenticationResult.IsAuthenticated)
            {
                return SignOut();
            }

            // OVO NE TREBA IMAŠ INFO U aCCESS TOKENU
            var refreshTokenUser = await _authenticationService.GetRefreshTokenUserAsync(tokenRequest.RefreshToken);

            var jwtToken = await _authenticationService.GenerateJwtTokenAsync(refreshTokenUser.UserName);

            authenticationResult.IsAuthenticated = true;
            authenticationResult.Token = jwtToken.Token;
            authenticationResult.RefreshToken = jwtToken.RefreshToken;

            return Ok(authenticationResult);
        }
    }
}
