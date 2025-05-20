using Models.DTO;
using Models.Enums;
using Models.Requests;
using Models.Responses;

namespace UI.Services
{
    public class UserService : IUserService
    {
        private readonly ICommunicationService _communicationService;
        private readonly ILoggingService _loggingService;

        public UserService(
            ICommunicationService communicationService, 
            ILoggingService loggingService)
        {
            _communicationService = communicationService;
            _loggingService = loggingService;
        }

        public async Task<UserDTO> GetUserDataAsync(string username)
        {
            var url = $"Users/{username}";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Get, url);

            try
            {
                var response = await _communicationService.SendRequestAsync<UserDTO>(request);

                return response;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new UserDTO();
            }
        }

        public async Task<bool> ChangeUserStatus(ChangeUserStatusRQ changeStatusRequest)
        {
            var url = DefineChangeStatusURL(changeStatusRequest);
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Put, url);

            try
            {
                var response = await _communicationService.SendRequestAsync<ResponseBase>(request);
                return true;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return false;
            }
        }

        private string DefineChangeStatusURL(ChangeUserStatusRQ request)
        {
            string url = string.Empty;

            switch (request.NewUserStatus)
            {
                case ItemState.Active:
                    url = $"Users/Activate/{request.UserName}";
                    break;
                case ItemState.Inactive:
                    url = $"Users/Deactivate/{request.UserName}";
                    break;
                default:
                    throw new Exception($"New User status not supported: {request.NewUserStatus}");
            }

            return url;
        }

        public async Task<IAsyncEnumerable<UserDTO>> GetUsersAsync(UserFilterRQ userFilter)
        {
            var url = "Users";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Post, url, userFilter);

            try
            {
                var response = await _communicationService.SendRequestAsync<IAsyncEnumerable<UserDTO>>(request);

                return response;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return AsyncEnumerable.Empty<UserDTO>();
            }
        }

        public async Task<bool> UpdateUserDataAsync(UserDTO userDTO)
        {
            var url = "Users/Update";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Put, url, userDTO);

            var isSuccessful = false;

            try
            {
                isSuccessful = await _communicationService.SendRequestAsync<bool>(request);
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
            }

            return isSuccessful;
        }

        public async Task<UserGridFilterDataRS> GetUserFilterBaseValues()
        {
            var url = "Users/GridFilterData";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Get, url);

            try
            {
                var response = await _communicationService.SendRequestAsync<UserGridFilterDataRS>(request);

                return response;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new UserGridFilterDataRS();
            }
        }
    }
}
