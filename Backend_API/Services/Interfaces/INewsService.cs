using Backend_API.Data.Models;
using Models.DTO;
using Models.Requests;

namespace Backend_API.Services
{
    public interface INewsService
    {
        Task<IEnumerable<News>> GetNewsAsync(IEnumerable<RetrieveNewsRQ> retrieveNewsRQs);

        IEnumerable<NewsDTO> MapNewsToDTOs(IEnumerable<News> news);
    }
}
