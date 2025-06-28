using Models.Requests;
using Models.Responses;

namespace UI.Services
{
    public interface INewsService
    {
        Task<RetrieveNewsRS> GetNewsAsync(IEnumerable<RetrieveNewsRQ> newsFilter);
    }
}
