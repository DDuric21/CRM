using Backend_API.Data.Model;
using Backend_API.Data.Repositories;
using Backend_API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;

namespace Backend_API.Controllers
{
    public class UserController : Controller
    {
        private readonly ICrmRepository _repository;
        private readonly IUserService _userService;

        public UserController(
            ICrmRepository crmRepository, 
            UserManager<User> userManager,
            IUserService userService)
        {
            _repository = crmRepository;
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

            var userDTO = new UserDTO();
            try
            {
                var userData = await _userService.GetUserDataAsync(username);

                if (userData == null)
                {
                    return Problem("No uzser found!");
                }

                userDTO = _userService.MapUserDataToDTO(userData);
            }
            catch (Exception ex)
            {
                //add loging
                return StatusCode(500, ex.Message);
            }

            return Ok(userDTO);
        }
    }
}
