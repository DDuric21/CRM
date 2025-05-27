using Models.Responses;

namespace UI.Services
{
    public interface ICommunicationService
    {
        Task<HttpRequestMessage> CreateRequestAsync(HttpMethod httpMethod, string url);

        Task<HttpRequestMessage> CreateRequestAsync<T>(HttpMethod httpMethod, string url, T requestBody, bool addHeaders = true);

        Task<T> SendRequestAsyncNew<T>(HttpRequestMessage request) where T : IApiResponse, new();

        Task<T> SendRequestAsync<T>(HttpRequestMessage request); 
        
        Task<T> SendAuthenticationRequestAsync<T>(HttpRequestMessage request);

        Task SendErrorLogToServerUsingJsAsync(Exception exception, string url = null);
    }
}
