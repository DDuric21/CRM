using Backend_API.Services;
using Microsoft.AspNetCore.Mvc;
using Models.Helpers;

namespace Backend_API.Controllers
{
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        [Route("/Roles")]
        public async Task<IActionResult> GetAllApplicableRoles()
        {
            try
            {
                var roles = _roleService.GetAllApplicableRoles();

                if (roles.IsNullOrEmpty())
                {
                    return Problem("Roles not found!");
                }

                var roleDTOs = _roleService.MapUserRolesToDTO(roles);

                return Ok(roleDTOs); ;
            }
            catch (Exception ex)
            {
                //add loging
                return StatusCode(500, ex.Message);
            }
        }
    }
}
