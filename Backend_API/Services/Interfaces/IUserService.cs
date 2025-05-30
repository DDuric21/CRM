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

        Task<UserDTO> GetUserDataAsync(string username);

        Task<UserData> GetUserDataByNameAsync(string username);

        Task<User> CreateNewUserAsync(UserDTO userDTO);

        Task<List<UserDTO>> GetUsersAsync(UserFilterRQ userFilter);

        Task<bool> UpdateUserDataAsync(UserDTO userDTO);

        Task<IdentityResult> DeactivateUserAsync(string username);

        Task<IdentityResult> ActivateUserAsync(string username);

        Task<UserGridFilterDataRS> GetUserFilterBaseValuesAsync();
    }
}
