using Backend_API.Data.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text;

namespace Backend_API.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly CrmDbContext _context;
        public GenericRepository(CrmDbContext context)
        {
            _context = context;
        }

        public async Task<string> DeleteByIdAsync(long id)
        {
            var entity = await _context.Set<T>().FindAsync(id);

            //entity can be null
            var model = typeof(T);
            var modelName = typeof(T).Name;

            var message = new StringBuilder();
            if (entity == null)
            {
                message.AppendFormat("No such {0}", modelName);
            }
            else
            {
                _context.Remove(entity);
                message.AppendFormat("{0} by ID: {1} successfully deleted", modelName, id);
            }

            return message.ToString();
        }

        public virtual IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            var dbset = _context.Set<T>();

            var select = dbset
                .AsNoTracking()
                .Where(predicate);

            return select;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(long id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T> InsertAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);

            return entity;
        }

        public async void Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
