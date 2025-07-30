using Backend_API.Data.DbContext;
using Backend_API.Data.Models;
using Microsoft.EntityFrameworkCore;
using Models.Enums;

namespace Backend_API.Data.Repositories.Implementations
{
    public class QueueActionRepository : GenericRepository<QueueAction>, IQueueActionRepository
    {
        public QueueActionRepository(CrmDbContext context) : base(context)
        {
        }

        public async Task AddAsync(QueueAction queueAction)
        {
            _context.QueueActions.Add(queueAction);
            await _context.SaveChangesAsync();
        }

        public async Task<QueueAction> DequeueAsync(CancellationToken cancellationToken)
        {
            // Transaction + row lock to avoid races
            using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);
            var queueAction = await _context.QueueActions
                .Where(j => j.StatusId == (int)QueueActionStatus.Pending)
                .OrderBy(j => j.DateCreated)
                .FirstOrDefaultAsync(cancellationToken);

            if (queueAction == null)
            {
                return null;
            }

            queueAction.StatusId = (int)QueueActionStatus.InProgress;
            queueAction.Attempts++;
            queueAction.LastAttemptedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            await tx.CommitAsync(cancellationToken);
            return queueAction;
        }

        public async Task UpdateAsync(QueueAction queueAction)
        {
            _context.QueueActions.Update(queueAction);
            await _context.SaveChangesAsync();
        }
    }
}
