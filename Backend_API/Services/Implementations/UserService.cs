using AutoMapper;
using Backend_API.Data.DataClasses;
using Backend_API.Data.Models;
using Backend_API.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.DTO;
using Models.Enums;
using Models.Helpers;
using Models.Requests;
using Models.Responses;
using System.Security.Claims;

namespace Backend_API.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly CrmUserManager _userManager;
        private readonly CrmRoleManager _roleManager;

        public UserService(
            IMapper mapper,
            CrmUserManager userManager,
            CrmRoleManager roleManager)
        {
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<UserData> GetUserDataAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user is null)
            {
                return null;
            }

            var rolesName = await _userManager.GetRolesAsync(user);
            var roles = await _roleManager.GetClaimsAsync(rolesName);

            var userData = new UserData
            {
                User = user,
                UserRoles = roles
            };

            return userData;
        }

        public async Task<User> CreateNewUserAsync(UserDTO userDTO)
        {
            var user = new User
            {
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Email = userDTO.UserEmail,                
            };

            var userResult = await _userManager.CreateAsync(user, userDTO.Password);

            if (!userResult.Succeeded)
            {
                return null;
            }

            if (!userDTO.UserRoles.IsNullOrEmpty())
            {
                var userRoles = userDTO.UserRoles.Select(x => x.RoleName);
                var rolesResult = await _userManager.AddToRolesAsync(user, userRoles);

                if (!rolesResult.Succeeded)
                {
                    return null;
                }
            }

            return user;
        }

        public async Task<List<UserData>> GetUsersAsync(UserFilterRQ userFilter)
        {
            var users = _userManager.Users;

            var filteredUsers = FilterUsers(users, userFilter);

            var usersData = new List<UserData>();

            foreach (var user in filteredUsers)
            {
                var rolesName = await _userManager.GetRolesAsync(user);

                var roles = rolesName.ToDictionary(x => new IdentityRole(x), x => Enumerable.Empty<Claim>());

                if (userFilter.UserRoles.IsNullOrEmpty() || userFilter.UserRoles.Intersect(rolesName).Any())
                {
                    usersData.Add(new UserData { User = user, UserRoles = roles });
                }
            }

            return usersData;
        }

        private IEnumerable<User> FilterUsers(IQueryable<User> users, UserFilterRQ userFilter)
        {
            if (!userFilter.UserStatuses.IsNullOrEmpty())
            {
                users = users.Where(x => userFilter.UserStatuses.Contains((ItemState)x.UserStatusID));
            }

            if (userFilter.FirstName != null)
            {
                users = users.Where(x => x.FirstName.StartsWith(userFilter.FirstName));
            }

            if (userFilter.LastName != null)
            {
                users = users.Where(x => x.LastName.StartsWith(userFilter.LastName));
            }

            if (userFilter.CreatedDateStart != null)
            {
                users = users.Where(x => x.DateCreated >= userFilter.CreatedDateStart.Value);
            }

            if (userFilter.CreatedDateEnd != null)
            {
                users = users.Where(x => x.DateCreated <= userFilter.CreatedDateEnd.Value);
            }

            return users.ToList();
        }

        public async Task<bool> UpdateUserDataAsync(UserData userData)
        {
            var user = await _userManager.FindByNameAsync(userData.User.UserName);
            if (user is null)
            {
                return false;
            }

            var oldRoles = await _userManager.GetRolesAsync(user);
            var rolesToAdd = userData.UserRoles.Keys
                .Where(x => !oldRoles.Contains(x.Name))
                .Select(x => x.Name);

            var rolesToRemove = oldRoles
                .Where(x => !rolesToAdd.Contains(x));

            MapUserDataForUpdate(userData.User, user);

            // this need to be done in a transaction
            try
            {
                var updateResult = await _userManager.UpdateAsync(user);

                var removingResult = rolesToRemove.IsNullOrEmpty()
                    ? IdentityResult.Success
                    : await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

                var addingResult = rolesToAdd.IsNullOrEmpty()
                    ? IdentityResult.Success
                    : await _userManager.AddToRolesAsync(user, rolesToAdd);

                var isSuccessful = updateResult.Succeeded 
                    && removingResult.Succeeded 
                    && addingResult.Succeeded;

                return isSuccessful;
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, nameof(UserService), ex.Message);
                throw;
            }
        }

        public async Task<IdentityResult> DeactivateUserAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user is null || user.UserStatusID == (int)ItemState.Inactive)
            {
                DynamicLogger.LogError(nameof(UserService), "User not found or is inactive");
                return new IdentityResult();
            }

            user.UserStatusID = (int)ItemState.Inactive;
            var deactivationResult = await _userManager.UpdateAsync(user);

            return deactivationResult;
        }

        public async Task<IdentityResult> ActivateUserAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user is null || user.UserStatusID == (int)ItemState.Active)
            {
                DynamicLogger.LogError(nameof(UserService), "User not found or is inactive");
                return new IdentityResult();
            }

            user.UserStatusID = (int)ItemState.Active;

            var deactivationResult = await _userManager.UpdateAsync(user);

            return deactivationResult;
        }

        public async Task<UserGridFilterDataRS> GetUserFilterBaseValuesAsync()
        {
            var roles = await _roleManager.Roles
                .Select(x => x.Name)
                .ToListAsync();

            var userStatuses = Enum.GetValues(typeof(ItemState)).Cast<ItemState>().ToList();

            var userFilterBaseValues = new UserGridFilterDataRS
            {
                UserRoles = roles,
                UserStatuses = userStatuses
            };

            return userFilterBaseValues;
        }

        #region mappings
        public UserDTO MapUserToDTO(User user)
        {
            return _mapper.Map<UserDTO>(user);
        }

        public UserDTO MapUserDataToDTO(UserData userData)
        {
            return _mapper.Map<UserDTO>(userData);
        }

        public List<UserDTO> MapUsersDataToDTOs(IEnumerable<UserData> users)
        {
            var mapedUsers = new List<UserDTO>();
            foreach (var user in users)
            {
                mapedUsers.Add(MapUserDataToDTO(user));
            }

            return mapedUsers;
        }

        public UserData MapDtoToUserData(UserDTO userDTO)
        {
            if (userDTO.IsNullOrEmpty())
            {
                return new UserData();
            }

            var userData = _mapper.Map<UserData>(userDTO);

            return userData;
        }

        private void MapUserDataForUpdate(User newUserData, User existingUserData)
        {
            existingUserData.FirstName = newUserData.FirstName;
            existingUserData.LastName = newUserData.LastName;
            existingUserData.Email = newUserData.Email;
            existingUserData.UserName = newUserData.UserName;
        }
        #endregion
    }
}
