using AutoMapper;
using Backend_API.Data.DataClasses;
using Backend_API.Data.Model;
using Models.DTO;

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

        public UserDTO MapUserToDTO(User user)
        {
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserData> GetUserDataAsync(string username)
        {
            var user = await _userManager.FindByEmailAsync(username);

            if (user is null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            var userData =  new UserData
            {
                User = user,
                UserRoles = roles.ToHashSet()
            };

            return userData;
        }

        public UserDTO MapUserDataToDTO(UserData userData)
        {
            return _mapper.Map<UserDTO>(userData);
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
    }
}
