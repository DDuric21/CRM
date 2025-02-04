using Backend_API.Data.DataClasses;
using Backend_API.Data.Model;
using Models.DTO;

namespace Backend_API.Services
{
    public interface IUserService
    {
        UserDTO MapUserToDTO(User user);

        UserDTO MapUserDataToDTO(UserData userData);

        Task<UserData> GetUserDataAsync(string username);

        Task<User> CreateNewUserAsync(UserDTO userDTO);

        Task<List<User>> GetAllUsersAsync();

        List<UserDTO> MapUsersToDTOs(IEnumerable<User> users);

        UserData MapDtoToUserData(UserDTO user);

        Task<bool> UpdateUserDataAsync(UserData userData);
    }
}
