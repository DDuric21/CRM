using Models.DTO;

namespace UI.Services
{
    public class UserService : IUserService
    {
        private readonly ICommunicationService _communicationService;
        private readonly ICrmModalService _modalService;

        public UserService(
            ICommunicationService communicationService, 
            ICrmModalService modalService)
        {
            _communicationService = communicationService;
            _modalService = modalService;
        }

        public async Task<UserDTO> GetUserDataAsync(string username)
        {
            var url = string.Format("https://localhost:7076/Users/{0}", username);
            var request = _communicationService.CreateRequest(HttpMethod.Get, url);

            try
            {
                var response = await _communicationService.SendRequestAsync<UserDTO>(request);

                return response;
            }
            catch (Exception ex)
            {
                // loging
                _modalService.ShowErrorMessage(ex.Message);

                return null;
            }
        }

        public async Task<int> DeactivateUser(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<IAsyncEnumerable<UserDTO>> GetUsersAsync()
        {
            var url = string.Format("https://localhost:7076/Users");
            var request = _communicationService.CreateRequest(HttpMethod.Get, url);

            try
            {
                var response = await _communicationService.SendRequestAsync<IAsyncEnumerable<UserDTO>>(request);

                return response;
            }
            catch (Exception ex)
            {
                // loging
                _modalService.ShowErrorMessage(ex.Message);

                return null;
            }
        }

        public async Task<bool> UpdateUserDataAsync(UserDTO userDTO)
        {
            var url = string.Format("https://localhost:7076/Users/Update");
            var request = _communicationService.CreateRequest(HttpMethod.Put, url, userDTO);

            var isSuccessful = false;

            try
            {
                isSuccessful = await _communicationService.SendRequestAsync<bool>(request);
            }
            catch (Exception ex)
            {
                // loging
                _modalService.ShowErrorMessage(ex.Message);
            }

            return isSuccessful;
        }
    }
}
