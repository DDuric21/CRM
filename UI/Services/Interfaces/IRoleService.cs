using Models.DTO;
using Models.Responses;
using UI.Helpers;

namespace UI.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<UserRoleDTO>> GetApplicableRolesAsync();

        Task<ActionResult<GetRolePermissionsRS>> GetRolePermissionsAsync(string username);

        Task<ActionResult<GetAllPermissionsRS>> GetAllPermissionsAsync();

        Task<ActionResult<object>> UpdateRolePermissionsAsync(string roleName, Dictionary<string, bool> permissions);

        Task<ActionResult<object>> DeleteRoleAsync(string roleName);

        Task<ActionResult<object>> CreateRoleAsync(RoleDTO roleDTO);
    }
}
