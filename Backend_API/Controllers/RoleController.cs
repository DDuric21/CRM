using Backend_API.Helpers;
using Backend_API.Services;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Models.Helpers;
using Models.Requests;
using Models.Responses;

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
            var roles = _roleService.GetAllApplicableRoles();

            if (roles.IsNullOrEmpty())
            {
                return Problem("Roles not found!");
            }

            return Ok(roles);
        }

        [HttpGet]
        [Route("Permissions")]
        public async Task<IActionResult> GetAllApplicablePermissions()
        {
            var permissions = await _roleService.GetAllApplicablePermissionsAsync();

            if (permissions.IsNullOrEmpty())
            {
                return Problem("Permissions not found!");
            }

            return Ok(permissions);
        }

        [HttpGet]
        [Route("Permissions/{roleName}")]
        public async Task<IActionResult> GetRolePermissions(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return HttpContext.BadRequest();
            }

            var response = await _roleService.GetRolePermissionsAsync(roleName);
            if (response is null)
            {
                return Problem("No permissions found for the specified role.");
            }

            return Ok(new GetRolePermissionsRS { RolePermissions = response });
        }

        [HttpPut]
        [Route("Permissions")]
        public async Task<IActionResult> UpdateRolePermissions([FromBody] RolePermissions rolePermissions)
        {
            if (rolePermissions.IsNullOrEmpty())
            {
                return HttpContext.BadRequest();
            }

            if (!await _roleService.UpdateRolePermissionsAsync(rolePermissions))
            {
                return Problem("No permissions updated");
            }

            return Ok();
        }

        [HttpDelete]
        [Route("Permissions/{roleName}")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return HttpContext.BadRequest();
            }

            if (!await _roleService.DeleteRoleAsync(roleName))
            {
                return Problem($"Role {roleName} not deleted!");
            }

            return Ok();
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateNewRole([FromBody] CreateNewRoleRQ createNewRoleRQ)
        {
            if (createNewRoleRQ.IsNullOrEmpty())
            {
                return HttpContext.BadRequest();
            }

            if (!await _roleService.CreateRoleAsync(createNewRoleRQ))
            {
                return Problem($"Role {createNewRoleRQ.RoleDTO.RoleName} not deleted!");
            }

            return Ok();
        }
    }
}
