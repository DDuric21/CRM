using Models.Authentication;
using Models.DTO;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using UI.Authentication;

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
                UserName = username,
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
                // logging
                _modalService.ShowErrorMessage(ex.Message);
                return null;
            }
        }

        public UserSession CreateUserSessionFromJwt(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("Invalid token provided");
            }

            var userSession = new UserSession();
            var jwt = DecodeJwt(token);

            if (jwt.TryGetValue(CrmJwtClaimNames.Name, out var userName))
            {
                userSession.UserName = userName.ToString();
            }

            userSession.Roles = ExtractJwtNodeValues(jwt, CrmJwtClaimNames.Role);
            userSession.Permissions = ExtractJwtNodeValues(jwt, CrmJwtClaimNames.Permission);

            return userSession;
        }

        private IEnumerable<string> ExtractJwtNodeValues(Dictionary<string, object> jwt, string nodeKey)
        {
            if (!jwt.TryGetValue(nodeKey, out var nodeValue) || nodeValue is null)
            {
                // add logging
                return Enumerable.Empty<string>();
            }

            var values = new List<string>();

            if (nodeValue is JsonElement jsonElement 
                && jsonElement.ValueKind == JsonValueKind.Array)
            {
                values = jsonElement.EnumerateArray()
                    .Select(r => r.ToString())
                    .ToList();
            }
            else
            {
                values.Add(nodeValue.ToString());
            }

            return values;
        }

        private Dictionary<string, object> DecodeJwt(string token)
        {
            var parts = token.Split('.');
            if (parts.Length != 3)
            {
                throw new ArgumentException("Invalid JWT format");
            }

            var payload = parts[1];
            var jsonBytes = Convert.FromBase64String(payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '='));
            var jwtData = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            return jwtData;
        }
    }
}
