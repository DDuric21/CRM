using Models.DTO;
using Models.Requests;
using Models.Responses;

namespace Backend_API.Services
{
    public interface IRoleService
    {
        HashSet<UserRoleDTO> GetAllApplicableRoles();

        Task<RolePermissions> GetRolePermissionsAsync(string roleName);

        Task<bool> UpdateRolePermissionsAsync(RolePermissions rolePermissions);

        Task<bool> DeleteRoleAsync(string roleName);

        Task<GetAllPermissionsRS> GetAllApplicablePermissionsAsync();

        Task<bool> CreateRoleAsync(CreateNewRoleRQ createNewRoleRQ);
    }
}
