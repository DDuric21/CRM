using Microsoft.JSInterop;
using Models.Authentication;
using Models.Helpers;
using System.Net.Http.Json;
using System.Text.Json;
using UI.Authentication;

namespace UI.Services
{
    public class CrmCommunicationService : ICommunicationService, IAsyncDisposable
    {
        private readonly AppConfig _apiConfig;
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;
        private IJSObjectReference? _module;

        public CrmCommunicationService(
            AppConfig apiConfig,
            HttpClient httpClient,
            IJSRuntime jsRuntime)
        {
            _apiConfig = apiConfig;
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }

        public async Task<HttpRequestMessage> CreateRequestAsync<T>(HttpMethod httpMethod, string target, T requestBody, bool addAuthorization = true)
        {
            var url = $"{_apiConfig.SecureBackendUrl}/{target}";
            var request = new HttpRequestMessage(httpMethod, url);

            await AddBasicHeaders(request, addAuthorization);
            
            var content = JsonContent.Create(requestBody);
            request.Content = content;

            return request;
        }

        public async Task<HttpRequestMessage> CreateRequestAsync(HttpMethod httpMethod, string target)
        {
            var url = $"{_apiConfig.SecureBackendUrl}/{target}";
            var request = new HttpRequestMessage(httpMethod, url);

            await AddBasicHeaders(request, true);

            return request;
        }

        public async Task<T> SendRequestAsync<T>(HttpRequestMessage request)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = string.Format("Server returned an error with status: {0} \r\n {1}", response.StatusCode, response.Content);
                throw new Exception(errorMessage);
            }

            var result = await response.Content.ReadAsStreamAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var deserialisedResult = await JsonSerializer.DeserializeAsync<T>(result, options: options);

            return deserialisedResult;
        }

        public async Task<T> SendAuthenticationRequestAsync<T>(HttpRequestMessage request)
        {
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = string.Format("Server returned an error with status: {0} \r\n {1}", response.StatusCode, response.Content);
                throw new Exception(errorMessage);
            }

            T deserialisedResult = default;
            if (typeof(T) == typeof(string))
            {
                var stringResult = await response.Content.ReadAsStringAsync();
                deserialisedResult = (T)(object)stringResult;
            }
            else
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var result = await response.Content.ReadAsStreamAsync();
                deserialisedResult = await JsonSerializer.DeserializeAsync<T>(result, options: options);
            }

            return deserialisedResult;
        }

        private async Task AddBasicHeaders(HttpRequestMessage request, bool addAuthorization = false)
        {
            request.Headers.Add("Accept", "application/json");

            if (addAuthorization)
            {
                var jwt = await GetAuthorizationToken();
                request.Headers.Add("Authorization", $"Bearer {jwt}");
            }
        }

        private async Task<string> GetAuthorizationToken()
        {
            if (JasonWebToken.IsExpired)
            {
                await RefreshAccesTokenAsync();
            }

            return JasonWebToken.Value;
        }

        private async Task RefreshAccesTokenAsync()
        {
            var url = $"{_apiConfig.SecureBackendUrl}/RefreshToken";
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            var requestBody = new RefreshTokenRQ
            {
                AccessToken = JasonWebToken.Value
            };

            var content = JsonContent.Create(requestBody);
            request.Content = content;

            try
            {
                var response = await SendAuthenticationRequestAsync<string>(request);

                if (response != null)
                {
                    JasonWebToken.Value = response;
                }
            }
            catch (Exception ex)
            {
                SendErrorLogToServerUsingJsAsync(ex);
            }
        }

        public async Task SendErrorLogToServerUsingJsAsync(Exception exception, string url = null)
        {
            if (_module == null)
            {
                _module = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                   "import", "./js/modules.js");
            }

            var path = url;
            var stackTrace = ExceptionHelper.FlattenExceptionMessages(exception);

            await _module.InvokeVoidAsync(
                "logExceptionToServer",
                _apiConfig.SecureBackendUrl,
                exception.Message,
                stackTrace,
                path,
                "Fatal");
        }

        public async ValueTask DisposeAsync()
        {
            if (_module is not null)
            {
                await _module.DisposeAsync();
            }
        }
    }
}
