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
                UserRole = roles.FirstOrDefault()
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
        #endregion
    }
}
