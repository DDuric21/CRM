using Models.DTO;
using Models.Requests;

namespace Backend_API.Services
{
    public interface INewsService
    {
        Task<IEnumerable<NewsDTO>> RetrieveNewsAsync(IEnumerable<RetrieveNewsRQ> retrieveNewsRQs);
    }
}
