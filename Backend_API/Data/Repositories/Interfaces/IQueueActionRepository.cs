using Backend_API.Data.Models;

namespace Backend_API.Data.Repositories
{
    public interface IQueueActionRepository : IGenericRepository<QueueAction>
    {
        Task AddAsync(QueueAction queueAction);

        Task<QueueAction> DequeueAsync(CancellationToken cancellationToken);

        Task UpdateAsync(QueueAction job);
    }
}
