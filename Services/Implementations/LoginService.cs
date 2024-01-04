using Backend_API.Data.DTO;
using Backend_API.Data.Repositories;
using Backend_API.HelperMethods;

namespace Backend_API.Services
{
    public class LoginService : ILoginService
    {

        private readonly ICrmRepository _repository;

        public LoginService(ICrmRepository crmRepository)
        {
            _repository = crmRepository;
        }

        public bool ValidateLogin(UserDTO userDTO)
        {
            var user = _repository.Users
                .Where(x => x.UserEmail.ToLower() == userDTO.UserEmail.ToLower())
                .FirstOrDefault();

            if (user.IsNullOrEmpty()
                || userDTO.Password != user.Password)
            {
                return false;
            }

            return true;
        }
    }
}
