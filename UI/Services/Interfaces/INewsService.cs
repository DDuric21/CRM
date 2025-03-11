using Models.DTO;
using Models.Requests;

namespace UI.Services
{
    public interface INewsService
    {
        Task<IEnumerable<NewsDTO>> GetNewsAsync(IEnumerable<RetrieveNewsRQ> newsFilter);
    }
}
