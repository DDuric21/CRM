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
        /// <returns>Task of type long that contains the id of inserted element</returns>
        Task<long> InsertAsync(T entity);

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
        /// <returns>Task of type string with information of deleted elements</returns>
        Task<string> DeleteByIdAsync(long id);

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
    }
}
