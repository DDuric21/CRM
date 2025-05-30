using AutoMapper;
using Backend_API.Data.DataClasses;
using Backend_API.Data.DbContext;
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
        private readonly CrmDbContext _crmDbContext;

        public UserService(
            IMapper mapper,
            CrmUserManager userManager,
            CrmRoleManager roleManager,
            CrmDbContext crmDbContext)
        {
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _crmDbContext = crmDbContext;
        }

        public async Task<UserDTO> GetUserDataAsync(string username)
        {
            var userData = await GetUserDataByNameAsync(username);

            try
            {
                var userDTO = _mapper.Map<UserDTO>(userData);

                return userDTO;
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, "Error mapping user data to DTO");
                throw;
            }
        }

        public async Task<UserData> GetUserDataByNameAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user is null)
            {
                return new UserData();
            }

            try
            {
                var rolesName = await _userManager.GetRolesAsync(user);

                var userData = new UserData
                {
                    User = user,
                    UserRoles = await _roleManager.GetClaimsAsync(rolesName)
                };

                return userData;
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, "User Claims not retrieved correctly");
                throw;
            }
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

        public async Task<List<UserDTO>> GetUsersAsync(UserFilterRQ userFilter)
        {
            try
            {
                var filteredUsers = FetchUsers(userFilter);
                var usersData = await FetchUsersRolesAsync(userFilter, filteredUsers);

                var usersDTOs = _mapper.Map<List<UserDTO>>(usersData);

                return usersDTOs;
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, "Error fetching users");
                throw;
            }
        }

        public async Task<bool> UpdateUserDataAsync(UserDTO userDTO)
        {
            var userData = MapToUserData(userDTO);
            if (userData.IsNullOrEmpty())
            {
                DynamicLogger.LogError("User data is invalid or missing username");
                return false;
            }

            var user = await _userManager.FindByNameAsync(userData.User.UserName);
            if (user is null)
            {
                return false;
            }

            SetUserDataForUpdate(userData.User, user);
            var updateUserData = await CreateUpdateUserData(userData, user);
            var isSuccess = await UpdateUserDataSafeAsync(updateUserData);

            return isSuccess;
        }

        private UserData MapToUserData(UserDTO userDTO)
        {
            try
            {
                var userData = _mapper.Map<UserData>(userDTO);
                userData.UserRoles = userDTO.UserRoles
                    .ToDictionary(
                        x => new IdentityRole(x.RoleName),
                        x => Enumerable.Empty<Claim>());

                return userData;
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, $"Error mapping {nameof(UserDTO)} to {nameof(UserData)}");
                throw;
            }
        }

        private async Task<bool> UpdateUserDataSafeAsync(UpdateUserData updateUserData)
        {
            var isSuccessful = false;
            await using var transaction = await _crmDbContext.Database.BeginTransactionAsync();
            try
            {
                var updateResult = await _userManager.UpdateAsync(updateUserData.User);

                var removingResult = updateUserData.RolesToRemove.IsNullOrEmpty()
                    ? IdentityResult.Success
                    : await _userManager.RemoveFromRolesAsync(updateUserData.User, updateUserData.RolesToRemove);

                var addingResult = updateUserData.RolesToAdd.IsNullOrEmpty()
                    ? IdentityResult.Success
                    : await _userManager.AddToRolesAsync(updateUserData.User, updateUserData.RolesToAdd);

                isSuccessful = updateResult.Succeeded
                    && removingResult.Succeeded
                    && addingResult.Succeeded;

                return isSuccessful;
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, "Failed updating user data");
                throw;
            }
            finally
            {
                if (isSuccessful)
                {
                    await transaction.CommitAsync();
                }
                else
                {
                    await transaction.RollbackAsync();
                }
            }
        }

        public async Task<IdentityResult> DeactivateUserAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user is null || user.UserStatusID == (int)ItemState.Inactive)
            {
                DynamicLogger.LogError("User not found or is inactive");
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
                DynamicLogger.LogError("User not found or is inactive");
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

        public UserDTO MapUserToDTO(User user)
        {
            return _mapper.Map<UserDTO>(user);
        }

        private async Task<List<UserData>> FetchUsersRolesAsync(UserFilterRQ userFilter, IEnumerable<User> filteredUsers)
        {
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

        private IEnumerable<User> FetchUsers(UserFilterRQ userFilter)
        {
            var users = _userManager.Users;
            var filteredUsers = FilterUsers(users, userFilter);

            return filteredUsers;
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

        private void SetUserDataForUpdate(User newUserData, User existingUserData)
        {
            existingUserData.FirstName = newUserData.FirstName;
            existingUserData.LastName = newUserData.LastName;
            existingUserData.Email = newUserData.Email;
            existingUserData.UserName = newUserData.UserName;
        }

        private async Task<UpdateUserData> CreateUpdateUserData(UserData userData, User user)
        {
            var rolesToEdit = userData.UserRoles.Keys
                .Select(x => x.Name)
                .ToList();

            var oldRoles = await _userManager.GetRolesAsync(user);

            var rolesToAdd = rolesToEdit
                .Where(x => !oldRoles.Contains(x))
                .ToList();

            var rolesToRemove = oldRoles
                .Where(x => rolesToEdit.Contains(x))
                .ToList();

            var updateUserData = new UpdateUserData
            {
                User = user,
                RolesToAdd = rolesToAdd,
                RolesToRemove = rolesToRemove
            };

            return updateUserData;
        }

        private class UpdateUserData
        {
            internal User User { get; set; }
            internal IEnumerable<string> RolesToAdd { get; set; }
            internal IEnumerable<string> RolesToRemove { get; set; }
        }
    }
}
