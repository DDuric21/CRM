using Backend_API.Helpers;
using Backend_API.Services;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Models.Helpers;
using Models.Requests;
using Models.Responses;
using Resources.Translations.API;

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
                return HttpContext.BadRequest();
            }

            var userData = await _userService.GetUserDataAsync(username);

            if (userData.IsNullOrEmpty())
            {
                return Problem("No user found!");
            }

            return Ok(userData);
        }

        [HttpPost]
        public async Task<IActionResult> GetUsers([FromBody] UserFilterRQ userFilter)
        {
            var users = await _userService.GetUsersAsync(userFilter);

            if (users is null)
            {
                return Problem(APITranslations.FetchingUsersFailed);
            }

            return Ok(users);
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> UpdateUserData([FromBody] UserDTO userDTO)
        {
            if (userDTO.IsNullOrEmpty())
            {
                return HttpContext.BadRequest();
            }

            var result = await _userService.UpdateUserDataAsync(userDTO);

            return Ok(result);
        }

        [HttpPut]
        [Route("Deactivate/{username}")]
        public async Task<IActionResult> DeactivateUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return HttpContext.BadRequest();
            }

            var result = await _userService.DeactivateUserAsync(username);

            if (!result.Succeeded)
            {
                return Problem("No user deactivated!");
            }

            return Ok(new ResponseBase(result.Succeeded));
        }

        [HttpPut]
        [Route("Activate/{username}")]
        public async Task<IActionResult> ActivateUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return HttpContext.BadRequest();
            }

            var result = await _userService.ActivateUserAsync(username);

            if (!result.Succeeded)
            {
                return Problem("No user activated!");
            }

            return Ok(new ResponseBase(result.Succeeded));
        }

        [HttpGet]
        [Route("GridFilterData")]
        public async Task<IActionResult> GetUserFilterBaseValues()
        {
            var result = await _userService.GetUserFilterBaseValuesAsync();

            if (result is null)
            {
                return Problem("No data fetched!");
            }

            return Ok(result);
        }
    }
}
