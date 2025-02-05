using Models.DTO;
using Models.Requests;

namespace UI.Services
{
    public interface IUserService
    {
        Task<UserDTO> GetUserDataAsync(string username);

        Task ChangeUserStatus(ChangeUserStatusRQ request);

        Task<IAsyncEnumerable<UserDTO>> GetUsersAsync();

        Task<bool> UpdateUserDataAsync(UserDTO userDTO);
    }
}
