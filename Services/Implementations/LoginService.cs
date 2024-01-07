using Backend_API.Data.DTO;
using Backend_API.HelperMethods;
using Microsoft.AspNetCore.Identity;

namespace Backend_API.Services
{
    public class LoginService : ILoginService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public LoginService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public bool ValidateLogin(UserDTO userDTO)
        {
            var user = _userManager.FindByEmailAsync(userDTO.UserEmail).Result;

            if (user.IsNullOrEmpty())
            {
                return false;
            }
            var isPasswordCorrect = _userManager.CheckPasswordAsync(user, userDTO.Password).Result;
            if (!isPasswordCorrect)
            {
                return false;
            }

            return true;
        }
    }
}
