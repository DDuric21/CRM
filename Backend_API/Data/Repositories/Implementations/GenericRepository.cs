using Backend_API.Data.DbContext;
using Microsoft.EntityFrameworkCore;
using Models.HelperMethods;
using System.Linq.Expressions;
using System.Text;

namespace Backend_API.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected CrmDbContext _context;
        public GenericRepository(CrmDbContext context)
        {
            _context = context;
        }

        public async Task<string> DeleteByIdAsync(long id)
        {
            var entity = await _context.Set<T>().FindAsync(id);

            //entity can be null
            var modelName = typeof(T).Name;

            var message = new StringBuilder();
            if (entity == null)
            {
                message.AppendFormat("No such {0}", modelName);
            }
            else
            {
                _context.Remove(entity);
                await _context.SaveChangesAsync();
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

        public async Task<long> InsertAsync(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();

            var idProperty = entity.GetType().GetProperty("Id")?.GetValue(entity);

            return idProperty.IsNullOrEmpty()
                ? 0L
                : (long)idProperty;
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(T entity)
        {
            //malo bolje ovo rješiti
            _context.Set<T>().Add(entity);
            _context.Entry(entity).State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public void Insert(T entity)
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();
        }
    }
}
