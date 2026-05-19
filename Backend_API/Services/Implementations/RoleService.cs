using AutoMapper;
using Backend_API.Data.Repositories;
using Backend_API.Logging;
using Microsoft.AspNetCore.Identity;
using Models.Authentication;
using Models.DTO;
using Models.Helpers;
using Models.Requests;
using Models.Responses;
using System.Security.Claims;

namespace Backend_API.Services
{
    public class RoleService : IRoleService
    {
        private readonly CrmRoleManager _roleManager;
        private readonly IMapper _mapper;
        private readonly ICrmRepository _crmRepository;

        public RoleService(
            CrmRoleManager roleManager,
            IMapper mapper,
            ICrmRepository crmRepository)
        {
            _roleManager = roleManager;
            _mapper = mapper;
            _crmRepository = crmRepository;
        }

        public HashSet<UserRoleDTO> GetAllApplicableRoles()
        {
            var roles = _roleManager.Roles
                .ToHashSet();

            try
            {
                var userRoleDTOs = _mapper.Map<IEnumerable<IdentityRole>, HashSet<UserRoleDTO>>(roles);
                return userRoleDTOs;
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, "Error while mapping object");
                throw;
            }
        }

        public async Task<RolePermissions> GetRolePermissionsAsync(string roleName)
        {
            var permissions = await _crmRepository.RolePermissions.GetAllAsync();
            var rolePermissions = await _roleManager.GetClaimsAsync(roleName);

            var rolePermissionsCombined = new RolePermissions
            {
                RoleName = roleName,
                Permissions = permissions.ToDictionary(
                    x => x.Name,
                    x => rolePermissions.Any(y => 
                        y.Type == CrmJwtClaimNames.Permission
                        && y.Value == x.Name))
            };
            
            return rolePermissionsCombined;
        }

        public async Task<bool> UpdateRolePermissionsAsync(RolePermissions rolePermissions)
        {
            var userRole = _roleManager.Roles
                .FirstOrDefault(x => x.Name == rolePermissions.RoleName);

            if (userRole is null)
            {
                throw new ArgumentException($"Role '{rolePermissions.RoleName}' does not exist.");
            }

            var permissionUpdateData = await CreateUserPermissionUpdateData(rolePermissions, userRole);

            var result = await UpdateUserPermissionsSafeAsync(permissionUpdateData);

            return result;
        }

        public async Task<bool> DeleteRoleAsync(string roleName)
        {
            try
            {
                return await _roleManager.DeleteRoleAsync(roleName);
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, $"Error while deleting role {roleName}");
                return false;
            }
        }

        public async Task<GetAllPermissionsRS> GetAllApplicablePermissionsAsync()
        {
            try
            {
                var rolePermissions = await _crmRepository.RolePermissions.GetAllAsync();

                var permissions = new GetAllPermissionsRS 
                { 
                    Permissions = rolePermissions.Select(x => x.Name) 
                };

                return permissions;
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, "Error while retrieving all applicable permissions");
                throw;
            }
        }

        public async Task<bool> CreateRoleAsync(CreateNewRoleRQ createNewRoleRQ)
        {
            if (createNewRoleRQ.RoleDTO.IsNullOrEmpty()
                || string.IsNullOrEmpty(createNewRoleRQ.RoleDTO.RoleName))
            {
                throw new ArgumentNullException("Role basic data not provided!");
            }

            var userRole = new IdentityRole(createNewRoleRQ.RoleDTO.RoleName);

            var permissions = createNewRoleRQ.RoleDTO.Permissions
                .Where(x => x.Value)
                .Select(x => new Claim(CrmJwtClaimNames.Permission, x.Key))
                .ToList();

            var result = await _roleManager.CreateRoleAsync(userRole, permissions);

            if (!result)
            {
                DynamicLogger.LogError($"Failed to create role: {createNewRoleRQ.RoleDTO.RoleName}");
            }
            else
            {
                DynamicLogger.LogInfo($"Role created successfully: {createNewRoleRQ.RoleDTO.RoleName}");
            }

            return result;
        }

        private async Task<bool> UpdateUserPermissionsSafeAsync(UpdateUserPermissionsData permissionUpdateData)
        {
            try
            {
                await _roleManager.AddClaimsAsync(permissionUpdateData.RoleName, permissionUpdateData.ClaimsToAdd);
                await _roleManager.RemoveClaimsAsync(permissionUpdateData.RoleName, permissionUpdateData.ClaimsToRemove);

                DynamicLogger.LogInfo($"User permissions updated for role: {permissionUpdateData.RoleName}");
                return true;
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, "Error while updating user permissions");
                return false;
            }
        }

        private async Task<UpdateUserPermissionsData> CreateUserPermissionUpdateData(RolePermissions rolePermissions, IdentityRole userRole)
        {
            var oldRolePermissions = await _roleManager.GetClaimsAsync(rolePermissions.RoleName);

            var perrmisionsToRemove = rolePermissions.Permissions
                .Where(x => !x.Value && oldRolePermissions.Select(y => y.Value).Contains(x.Key))
                .Select(x => x.Key)
                .ToList();

            var perrmisionsToAdd = rolePermissions.Permissions
                .Where(x => x.Value && !oldRolePermissions.Select(y => y.Value).Contains(x.Key))
                .Select(x => x.Key)
                .ToList();

            var updateData = new UpdateUserPermissionsData
            {
                RoleName = rolePermissions.RoleName,
                ClaimsToAdd = perrmisionsToAdd.Select(x => new Claim(CrmJwtClaimNames.Permission, x)),
                ClaimsToRemove = perrmisionsToRemove.Select(x => new Claim(CrmJwtClaimNames.Permission, x))
            };

            return updateData;
        }

        private class UpdateUserPermissionsData
        {
            public string RoleName { get; set; }
            public IEnumerable<Claim> ClaimsToAdd = new List<Claim>();
            public IEnumerable<Claim> ClaimsToRemove = new List<Claim>();
        }
    }
}
