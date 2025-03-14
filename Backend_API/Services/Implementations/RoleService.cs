using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Models.DTO;

namespace Backend_API.Services
{
    public class RoleService : IRoleService
    {
        private readonly CrmRoleManager _roleManager;
        private readonly IMapper _mapper;

        public RoleService(
            CrmRoleManager roleManager,
            IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public HashSet<IdentityRole> GetAllApplicableRoles()
        {
            var roles = _roleManager.Roles
                .ToHashSet();

            return roles;
        }

        public HashSet<UserRoleDTO> MapUserRolesToDTO(IEnumerable<IdentityRole> userRoles)
        {
            var userRoleDTOs = _mapper.Map<IEnumerable<IdentityRole>, HashSet<UserRoleDTO>>(userRoles);

            return userRoleDTOs;
        }
    }
}
