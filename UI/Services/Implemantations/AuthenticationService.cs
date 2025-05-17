using Blazored.LocalStorage;
using Models.Authentication;
using Models.DTO;
using System.Net.Http.Json;
using UI.Authentication;

namespace UI.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ICrmModalService _modalService;
        private readonly ICommunicationService _communicationService;
        private readonly ILocalStorageService _localStorageService;
        private const string accessTokenKey = "accessToken_";

        public AuthenticationService(
            ICrmModalService modalService,
            ICommunicationService communicationService,
            ILocalStorageService localStorageService)
        {
            _modalService = modalService;
            _communicationService = communicationService;
            _localStorageService = localStorageService;
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
            var content = JsonContent.Create(userDto);
            try
            {
                var response = await _communicationService.SendAuthenticationRequestAsync<AuthenticationResult>(request);

                return response;
            }
            catch (Exception ex)
            {
                // logging
                _modalService.ShowErrorMessage(ex.Message);
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
                // logging
                _modalService.ShowErrorMessage(ex.Message);
                return null;
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
                // logging
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
                // logging
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
                // logging
            }
        }
    }
}
