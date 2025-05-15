using Backend_API.Logging;
using Backend_API.Services;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Models.Helpers;
using Models.Requests;
using Models.Responses;

namespace Backend_API.Controllers
{
    [Route("Users")]
    public class UserController : AuthorizationController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("{username}")]
        public async Task<IActionResult> GetUserData(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest();
            }

            try
            {
                var userData = await _userService.GetUserDataAsync(username);

                if (userData == null)
                {
                    return Problem("No user found!");
                }

                var userDTO = _userService.MapUserDataToDTO(userData);

                return Ok(userDTO);
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetUsers([FromBody] UserFilterRQ userFilter)
        {
            try
            {
                var users = await _userService.GetUsersAsync(userFilter);

                if (users.IsNullOrEmpty())
                {
                    return Problem("No users found!");
                }

                var userDTO = _userService.MapUsersDataToDTOs(users);

                return Ok(userDTO);
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> UpdateUserData([FromBody] UserDTO userDTO)
        {
            if (userDTO.IsNullOrEmpty())
            {
                return BadRequest();
            }

            try
            {
                var userData = _userService.MapDtoToUserData(userDTO);

                var result = await _userService.UpdateUserDataAsync(userData);

                return Ok(result);
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Route("Deactivate/{username}")]
        public async Task<IActionResult> DeactivateUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest();
            }

            try
            {
                var result = await _userService.DeactivateUserAsync(username);

                if (!result.Succeeded)
                {
                    return Problem("No user deactivated!");
                }

                return Ok(new ResponseBase());
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Route("Activate/{username}")]
        public async Task<IActionResult> ActivateUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest();
            }

            try
            {
                var result = await _userService.ActivateUserAsync(username);

                if (!result.Succeeded)
                {
                    return Problem("No user activated!");
                }

                return Ok(new ResponseBase());
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("GridFilterData")]
        public async Task<IActionResult> GetUserFilterBaseValues()
        {
            try
            {
                var result = await _userService.GetUserFilterBaseValuesAsync();

                if (result is null)
                {
                    return Problem("No data fetched!");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
