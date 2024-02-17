using Microsoft.AspNetCore.Mvc;
using Models.Authentication;
using Models.DTO;
using System.Net.Http.Headers;
using System.Text.Json;

namespace CRM_UI.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public ActionResult<AuthenticationResult> Login(string username, string password)
        {
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7076/Login");
            var mediaType = new MediaTypeHeaderValue("application/json");
            var userDto = new UserDTO
            {
                UserEmail = username,
                Password = password
            };
            var objectType = userDto.GetType();
            var content = JsonContent.Create(userDto, objectType, mediaType);

            request.Content = content;
            var response = httpClient.Send(request);

            var result = response.Content.ReadAsStream();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var deserialisedResult = JsonSerializer.Deserialize<AuthenticationResult>(result, options: options);

            return deserialisedResult;
        }
    }
}
