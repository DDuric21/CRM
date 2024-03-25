namespace UI.Services
{
    public interface ICommunicationService
    {
        HttpRequestMessage CreateRequest(HttpMethod httpMethod, string url);

        HttpRequestMessage CreateRequest<T>(HttpMethod httpMethod, string url, T requestBody);

        Task<T> SendRequestAsync<T>(HttpRequestMessage request);
    }
}
