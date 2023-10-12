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
        /// <returns>Inserted task of type TResult</returns>
        Task<T> InsertAsync(T entity);

        /// <summary>
        /// Deletes all elements for given id asynchronously
        /// </summary>
        /// <returns>Task of type string with information of deleted items</returns>
        Task<string> DeleteByIdAsync(long id);

        /// <summary>
        /// Saves all elements asynchronously
        /// </summary>
        /// <returns>Task</returns>
        void Save();

        /// <summary>
        /// Filters a sequence of values based on a predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>An System.Linq.IQueryable`1 that contains elements from the input sequence that satisfy the condition specified by predicate.</returns>
        /// <exceptions>T:System.ArgumentNullException: source or predicate is null.</exceptions>
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);
    }
}
