using Backend_API.Data.DTO;

namespace Backend_API.Services
{
    public interface ILoginService
    {
        /// <summary>
        /// Validates wether a user has the permission to login
        /// </summary>
        /// <param name="userDTO">User that requested login</param>
        /// <returns>True if has perrmision, False if not</returns>
        bool ValidateLogin(UserDTO userDTO);
    }
}
