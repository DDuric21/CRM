using Microsoft.JSInterop;
using Models.Authentication;
using Models.Authentication.DataStructures;
using Models.Helpers;
using Models.Responses;
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

        public async Task<T> SendRequestAsyncNew<T>(HttpRequestMessage request) where T : IApiResponse, new()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.SendAsync(request);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = await ExtractErrorMessageAsync(options, response);

                throw new Exception(errorMessage);
            }

            if (response.Content.Headers.ContentLength == 0)
            {
                return new T { IsSuccess = true };
            }

            var deserialisedResult = await DeserialiseResponseAsync<T>(options, response);

            return deserialisedResult;
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

        public async ValueTask DisposeAsync()
        {
            if (_module is not null)
            {
                await _module.DisposeAsync();
            }
        }

        private async Task AddBasicHeaders(HttpRequestMessage request, bool addAuthorization = false)
        {
            request.Headers.Add(HttpHeaderNames.Accept, "application/json");

            // this would be best to set at the begging of the action e.g. button clicked
            request.Headers.Add(HttpHeaderNames.CorrelationID, Guid.NewGuid().ToString());

            if (addAuthorization)
            {
                var jwt = await GetAuthorizationToken();
                request.Headers.Add(HttpHeaderNames.Authorization, $"Bearer {jwt}");
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

        private async Task<string> ExtractErrorMessageAsync(JsonSerializerOptions options, HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!_apiConfig.IsDevelopment())
            {
                return GetProductionErrorMessage(response);
            }

            try
            {
                var problem = JsonSerializer.Deserialize<ProblemDetails>(responseContent, options);
                if (!string.IsNullOrWhiteSpace(problem?.Detail))
                {
                    return problem.Detail;

                }
                else if (!string.IsNullOrWhiteSpace(problem?.Title))
                {
                    return problem.Title;
                }
            }
            catch
            {
                if (!string.IsNullOrWhiteSpace(responseContent))
                {
                    return responseContent;
                }
            }

            return $"HTTP {response.StatusCode}";
        }

        private async Task<T> DeserialiseResponseAsync<T>(JsonSerializerOptions options, HttpResponseMessage response)
        {
            T deserialisedResult = default;
            try
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                if ()
                {

                }
                deserialisedResult = JsonSerializer.Deserialize<T>(responseContent, options: options);
            }
            catch (Exception ex)
            {
                await SendErrorLogToServerUsingJsAsync(ex, response.RequestMessage?.RequestUri?.ToString());

                if (_apiConfig.IsDevelopment())
                {
                    throw;
                }

                throw new Exception(GetProductionErrorMessage(response));
            }

            if (deserialisedResult == null)
            {
                await SendErrorLogToServerUsingJsAsync(new Exception("Deserialised result is null."), response.RequestMessage?.RequestUri?.ToString());
                if (_apiConfig.IsDevelopment())
                {
                    throw new Exception("Deserialised result is null.");
                }

                throw new Exception(GetProductionErrorMessage(response));
            }

            return deserialisedResult;
        }

        private string GetProductionErrorMessage(HttpResponseMessage response)
        {
            var headerExists = response.Headers.TryGetValues(HttpHeaderNames.CorrelationID, out var correlationId);

            return headerExists
                ? $"An error occurred. Please contact support with the following correlation ID: {correlationId.FirstOrDefault()}"
                : "An error occurred. Please contact support.";
        }
    }
}
