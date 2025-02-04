using Models.DTO;

namespace UI.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<UserRoleDTO>> GetApplicableRolesAsync();
    }
}
