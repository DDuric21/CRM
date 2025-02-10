using Models.DTO;
using Models.Enums;
using Models.Requests;
using Models.Responses;

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

        public async Task ChangeUserStatus(ChangeUserStatusRQ changeStatusRequest)
        {
            var url = DefineChangeStatusURL(changeStatusRequest);
            var request = _communicationService.CreateRequest(HttpMethod.Put, url);

            try
            {
                var response = await _communicationService.SendRequestAsync<ResponseBase>(request);
            }
            catch (Exception ex)
            {
                // loging
                _modalService.ShowErrorMessage(ex.Message);
            }
        }

        private string DefineChangeStatusURL(ChangeUserStatusRQ request)
        {
            string url = string.Empty;

            switch (request.NewUserStatus)
            {
                case ItemState.Active:
                    url = string.Format("https://localhost:7076/Users/Activate/{0}", request.UserName);
                    break;
                case ItemState.Inactive:
                    url = string.Format("https://localhost:7076/Users/Deactivate/{0}", request.UserName);
                    break;
                default:
                    throw new Exception($"New User status not supported: {request.NewUserStatus}");
            }

            return url;
        }

        public async Task<IAsyncEnumerable<UserDTO>> GetUsersAsync(UserFilterRQ userFilter)
        {
            var url = string.Format("https://localhost:7076/Users");
            var request = _communicationService.CreateRequest(HttpMethod.Post, url, userFilter);

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

        public async Task<UserGridFilterDataRS> GetUserFilterBaseValues()
        {
            var url = string.Format("https://localhost:7076/Users/GridFilterData");
            var request = _communicationService.CreateRequest(HttpMethod.Get, url);

            try
            {
                var response = await _communicationService.SendRequestAsync<UserGridFilterDataRS>(request);

                return response;
            }
            catch (Exception ex)
            {
                // loging
                _modalService.ShowErrorMessage(ex.Message);

                return null;
            }
        }
    }
}
