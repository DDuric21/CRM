using Models.Authentication;
using Models.DTO;
using System.Net.Http.Json;

namespace UI.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ICrmModalService _modalService;
        private readonly ICommunicationService _communicationService;

        public AuthenticationService(
            ICrmModalService modalService,
            ICommunicationService communicationService)
        {
            _modalService = modalService;
            _communicationService = communicationService;
        }

        public async Task<AuthenticationResult> Login(string username, string password)
        {
            var httpClient = new HttpClient();
            var userDto = new UserDTO
            {
                UserEmail = username,
                Password = password
            };

            var response = await httpClient.PostAsJsonAsync("https://localhost:7076/Login", userDto);

            var deserializedResponse = new AuthenticationResult();

            if (!response.IsSuccessStatusCode)
            {
                return deserializedResponse;
            }

            try
            {
                deserializedResponse = await response.Content.ReadFromJsonAsync<AuthenticationResult>();

                return deserializedResponse;
            }
            catch (Exception ex)
            {
                _modalService.ShowErrorMessage(ex.Message);
            }

            return new AuthenticationResult();
        }

        public async Task<UserDTO> RegisterNewUserAsync(UserDTO userDTO)
        {
            var url = string.Format("https://localhost:7076/Register");
            var request = _communicationService.CreateRequest(HttpMethod.Post, url, userDTO);

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
    }
}
