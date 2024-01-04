using Backend_API.Data.DTO;
using Backend_API.HelperMethods;
using Backend_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend_API.Controllers
{
    public class LoginController : Controller
    {

        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpGet]
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

            return Ok();
        }
    }
}
