using System.Linq.Expressions;

namespace Backend_API.Data.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Gets all elements asynchronously
        /// </summary>
        /// <returns>Task of type TResult</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Gets elements for given Id asynchronously
        /// </summary>
        /// <returns>Task of type TResult</returns>
        Task<T> GetByIdAsync(long id);

        /// <summary>
        /// Inserts an element asynchronously
        /// </summary>
        Task InsertAsync(T entity);

        /// <summary>
        /// Inserts multiple elements all at once
        /// </summary>
        Task InsertRange(IEnumerable<T> entities);

        /// <summary>
        /// Inserts an element synchronously
        /// </summary>
        void Insert(T entity);

        /// <summary>
        /// Updates existing element for given Id asynchronously
        /// </summary>
        /// <param name="entity">Element to be updated</param>
        /// <returns>Task of type int that represents number of modified elements</returns>
        Task<int> UpdateAsync(T entity);

        /// <summary>
        /// Deletes all elements for given id asynchronously
        /// </summary>
        /// <returns>Task of type int with number of deleted elements</returns>
        Task<int> DeleteByIdAsync(long id);

        /// <summary>
        /// Saves all elements asynchronously
        /// </summary>
        /// <returns>Task of type int that represents number of saved elements</returns>
        Task<int> SaveAsync();

        /// <summary>
        /// Filters a sequence of values based on a predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>An System.Linq.IQueryable`1 that contains elements from the input sequence that satisfy the condition specified by predicate.</returns>
        /// <exceptions>T:System.ArgumentNullException: source or predicate is null.</exceptions>
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Filters a sequence of values based on a filter and includes data from connected objects
        /// </summary>
        /// <param name="filter">Expression of connditions for data</param>
        /// <param name="include">Expression of tables to be included when fetching data</param>
        /// <returns>An System.Linq.IQueryable`1 that contains elements from the input sequence that satisfy 
        /// the conditions and includes related objects for given expression</returns>
        IQueryable<T> Filter(Expression<Func<T, bool>> filter, Expression<Func<T, object>> include);

        IQueryable<T> With(Expression<Func<T, object>> include);
    }
}
