using Models.DTO;

namespace UI.Services
{
    public interface IUserService
    {
        Task<UserDTO> GetUserDataAsync(string username);
    }
}
