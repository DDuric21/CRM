using AutoMapper;
using Backend_API.Data.Models;
using Backend_API.Data.Repositories;
using Backend_API.Logging;
using Microsoft.EntityFrameworkCore;
using Models.DTO;
using Models.Requests;

namespace Backend_API.Services
{
    public class NewsService : INewsService
    {
        private readonly ICrmRepository _repository;
        private readonly IMapper _mapper;

        public NewsService(
            ICrmRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<NewsDTO>> RetrieveNewsAsync(IEnumerable<RetrieveNewsRQ> retrieveNewsRQs)
        {
            try
            {
                var news = await GetNewsAsync(retrieveNewsRQs);
                var newsDTOs = _mapper.Map<IEnumerable<NewsDTO>>(news);

                return newsDTOs;
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
                throw;
            }
        }

        private async Task<IEnumerable<News>> GetNewsAsync(IEnumerable<RetrieveNewsRQ> retrieveNewsRQs)
        {
            var news = new List<News>();

            var newsLimits = retrieveNewsRQs
                .GroupBy(x => x.NewsType)
                .ToDictionary(y => (int)y.Key, y => y.Sum(x => x.Amount));

            int batchSize = 50;
            int skip = 0;
            while (newsLimits.Any())
            {
                var batch = await _repository.News
                    .Where(x => newsLimits.Keys.Contains(x.NewsTypeID))
                    .OrderByDescending(x => x.DateCreated)
                    .Skip(skip)
                    .Take(batchSize)
                    .ToListAsync();

                if (!batch.Any())
                {
                    break;
                }

                foreach (var item in batch)
                {
                    if (newsLimits.TryGetValue(item.NewsTypeID, out int limit) && limit > 0)
                    {
                        news.Add(item);
                        newsLimits[item.NewsTypeID]--;

                        if (newsLimits[item.NewsTypeID] == 0)
                        {
                            newsLimits.Remove(item.NewsTypeID);
                        }
                    }
                }

                skip += batchSize;
            }

            return news;
        }
    }
}
