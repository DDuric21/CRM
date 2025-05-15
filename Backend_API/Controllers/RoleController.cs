using Backend_API.Logging;
using Backend_API.Services;
using Microsoft.AspNetCore.Mvc;
using Models.Helpers;

namespace Backend_API.Controllers
{
    [Route("Roles")]
    public class RoleController : AuthorizationController
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
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
                DynamicLogger.LogException(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
