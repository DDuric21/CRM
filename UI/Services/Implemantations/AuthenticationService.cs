using Models.Authentication;
using Models.DTO;
using System.Net.Http.Json;

namespace UI.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ICrmModalService _modalService;

        public AuthenticationService(ICrmModalService modalService)
        {
            _modalService = modalService;
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
    }
}
