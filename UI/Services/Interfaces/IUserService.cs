using Models.DTO;
using Models.Requests;
using Models.Responses;
using UI.Data;
using UI.Helpers;

namespace UI.Services
{
    public interface IUserService
    {
        Task<UserDTO> GetUserDataAsync(string username);

        Task<bool> ChangeUserStatus(ChangeUserStatusRQ request);

        Task<IAsyncEnumerable<UserDTO>> GetUsersAsync(UserFilterRQ userFilter);

        Task<ActionResult<object>> UpdateUserDataAsync(UserDTO userDTO, IEnumerable<SelectedUserRole> selectedUserRoles);

        Task<UserGridFilterDataRS> GetUserFilterBaseValues();
    }
}
