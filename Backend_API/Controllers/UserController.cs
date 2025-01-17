using Backend_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend_API.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("/Users/{username}")]
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
                //add loging
                return StatusCode(500, ex.Message);
            }
        }
    }
}
