using Microsoft.AspNetCore.Identity;
using Models.DTO;

namespace Backend_API.Services
{
    public interface IRoleService
    {
        HashSet<UserRoleDTO> MapUserRolesToDTO(IEnumerable<IdentityRole> userRoles);

        HashSet<IdentityRole> GetAllApplicableRoles();
    }
}
