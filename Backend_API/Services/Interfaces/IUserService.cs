using Backend_API.Data.DataClasses;
using Backend_API.Data.Models;
using Microsoft.AspNetCore.Identity;
using Models.DTO;
using Models.Requests;
using Models.Responses;

namespace Backend_API.Services
{
    public interface IUserService
    {
        UserDTO MapUserToDTO(User user);

        UserDTO MapUserDataToDTO(UserData userData);

        Task<UserData> GetUserDataAsync(string username);

        Task<User> CreateNewUserAsync(UserDTO userDTO);

        Task<List<UserData>> GetUsersAsync(UserFilterRQ userFilter);

        List<UserDTO> MapUsersDataToDTOs(IEnumerable<UserData> users);

        UserData MapDtoToUserData(UserDTO user);

        Task<bool> UpdateUserDataAsync(UserData userData);

        Task<IdentityResult> DeactivateUserAsync(string username);

        Task<IdentityResult> ActivateUserAsync(string username);

        Task<UserGridFilterDataRS> GetUserFilterBaseValuesAsync();
    }
}
