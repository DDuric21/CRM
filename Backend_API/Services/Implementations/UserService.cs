using AutoMapper;
using Backend_API.Data.DataClasses;
using Backend_API.Data.Model;
using Microsoft.AspNetCore.Identity;
using Models.DTO;
using Models.Helpers;

namespace Backend_API.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly CrmUserManager _userManager;

        public UserService(
            IMapper mapper,
            CrmUserManager userManager)
        {
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<UserData> GetUserDataAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user is null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            var userData =  new UserData
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

            var result = await _userManager.CreateAsync(user, userDTO.Password);

            if (!result.Succeeded)
            {
                return null;
            }

            return user;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();

            return users;
        }

        public async Task<bool> UpdateUserDataAsync(UserData userData)
        {
            var user = await _userManager.FindByNameAsync(userData.User.UserName);
            if (user is null)
            {
                return false;
            }

            var oldRoles = await _userManager.GetRolesAsync(user);
            var rolesToAdd = userData.UserRoles
                .Where(x => !oldRoles.Contains(x));
            var rolesToRemove = oldRoles
                .Where(x => !userData.UserRoles.Contains(x));

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
                //add loging
                throw;
            }
        }

        private void MapUserDataForUpdate(User newUserData, User existingUserData)
        {
            existingUserData.FirstName = newUserData.FirstName;
            existingUserData.LastName = newUserData.LastName;
            existingUserData.Email = newUserData.Email;
            existingUserData.UserName = newUserData.UserName;
        }

        #region mapings
        public UserDTO MapUserToDTO(User user)
        {
            return _mapper.Map<UserDTO>(user);
        }

        public UserDTO MapUserDataToDTO(UserData userData)
        {
            return _mapper.Map<UserDTO>(userData);
        }

        public List<UserDTO> MapUsersToDTOs(IEnumerable<User> users)
        {
            var mapedUsers = new List<UserDTO>();
            foreach (var user in users)
            {
                mapedUsers.Add(MapUserToDTO(user));
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
        #endregion
    }
}
