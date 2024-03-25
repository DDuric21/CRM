using System.Net.Http.Json;
using System.Text.Json;

namespace UI.Services
{
    public class CommunicationService : ICommunicationService
    {
        public HttpRequestMessage CreateRequest<T>(HttpMethod httpMethod, string url, T requestBody)
        {
            var request = new HttpRequestMessage(httpMethod, url);

            var content = JsonContent.Create(requestBody);
            request.Content = content;

            return request;
        }

        public HttpRequestMessage CreateRequest(HttpMethod httpMethod, string url)
        {
            var request = new HttpRequestMessage(httpMethod, url);

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
    }
}
