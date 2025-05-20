using Blazored.LocalStorage;
using Models.Authentication;
using Models.DTO;
using UI.Authentication;

namespace UI.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ICommunicationService _communicationService;
        private readonly ILocalStorageService _localStorageService;
        private readonly ILoggingService _loggingService;
        private const string accessTokenKey = "accessToken_";

        public AuthenticationService(
            ICommunicationService communicationService,
            ILocalStorageService localStorageService,
            ILoggingService loggingService)
        {
            _communicationService = communicationService;
            _localStorageService = localStorageService;
            _loggingService = loggingService;
        }

        public async Task<AuthenticationResult> Login(string username, string password)
        {
            var userDto = new UserDTO
            {
                UserName = username,
                Password = password
            };

            var url = "Login";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Post, url, userDto, false);

            try
            {
                var response = await _communicationService.SendAuthenticationRequestAsync<AuthenticationResult>(request);

                return response;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new AuthenticationResult();
            }
        }

        public async Task<UserDTO> RegisterNewUserAsync(UserDTO userDTO)
        {
            var url = "Register";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Post, url, userDTO);

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

        public UserSession CreateUserSession()
        {
            if (string.IsNullOrWhiteSpace(JasonWebToken.Value))
            {
                throw new ArgumentException("Invalid token provided");
            }

            var userSession = new UserSession();

            userSession.UserName = JasonWebToken.ReadValue(CrmJwtClaimNames.Name)
                .FirstOrDefault();

            userSession.Roles = JasonWebToken.ReadValue(CrmJwtClaimNames.Role);
            userSession.Permissions = JasonWebToken.ReadValue(CrmJwtClaimNames.Permission);

            return userSession;
        }

        public async Task ReadAccessTokenFromLocalStorageAsync()
        {
            try
            {
                var token = await _localStorageService.ReadEncryptedItemAsync<string>(accessTokenKey);
                JasonWebToken.Value = token;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
            }
        }

        public async Task SaveAccessTokenToLocalStorageAsync(string token)
        {
            try
            {
                await _localStorageService.SaveItemEncryptedAsync(accessTokenKey, token);
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
            }
        }

        public async Task RemoveAccessTokenFromLocalStorageAsync()
        {
            try
            {
                await _localStorageService.RemoveItemAsync(accessTokenKey);
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
            }
        }
    }
}
