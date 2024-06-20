using Backend_API.Data.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Backend_API.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected CrmDbContext _context;

        public GenericRepository(CrmDbContext context)
        {
            _context = context;
        }

        public async Task<int> DeleteByIdAsync(long id)
        {
            var entity = await _context.Set<T>().FindAsync(id);

            if (entity == null)
            {
                var modelName = typeof(T).Name;
                throw new Exception(string.Format("No such {0}", modelName));
            }
            else
            {
                _context.Remove(entity);
                return await _context.SaveChangesAsync();
            }
        }

        public virtual IQueryable<T> Filter(Expression<Func<T, bool>> filter, Expression<Func<T, object>> include)
        {
            var dbset = _context.Set<T>();

            var select = dbset
                .AsNoTracking()
                .Where(filter)
                .Include(include);

            return select;
        }

        public virtual IQueryable<T> With(Expression<Func<T, object>> include)
        {
            var dbset = _context.Set<T>();

            var select = dbset
                .AsNoTracking()
                .Include(include);

            return select;
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

        public async Task InsertAsync(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task InsertRange(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
            await _context.SaveChangesAsync();
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
