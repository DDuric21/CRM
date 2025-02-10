using Models.DTO;
using Models.Requests;
using Models.Responses;

namespace UI.Services
{
    public interface IUserService
    {
        Task<UserDTO> GetUserDataAsync(string username);

        Task ChangeUserStatus(ChangeUserStatusRQ request);

        Task<IAsyncEnumerable<UserDTO>> GetUsersAsync(UserFilterRQ userFilter);

        Task<bool> UpdateUserDataAsync(UserDTO userDTO);

        Task<UserGridFilterDataRS> GetUserFilterBaseValues();
    }
}
