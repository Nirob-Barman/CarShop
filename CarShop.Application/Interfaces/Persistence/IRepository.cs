using System.Linq.Expressions;

namespace CarShop.Application.Interfaces.Persistence
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(object id);
        Task<TResult?> GetByIdAsync<TResult>(object id, Expression<Func<T, TResult>> selector);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<T, TResult>> selector);
        Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector);
        Task<IEnumerable<TResult>> GetAllWithIncludesAsync<TResult>(Expression<Func<T, TResult>> selector, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<TResult>> GetAllWithIncludesAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize);
        Task<IEnumerable<TResult>> GetPagedAsync<TResult>(int pageNumber, int pageSize, Expression<Func<T, TResult>> selector);
        Task<IEnumerable<TResult>> GetPagedAsync<TResult>(int pageNumber, int pageSize, Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector);
        Task<IEnumerable<T>> Where(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<TResult>> Where<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector);
        Task<IEnumerable<TResult>> GetDistinctAsync<TResult>(Expression<Func<T, TResult>> selector);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<TResult?> FirstOrDefaultAsync<TResult>(Expression<Func<T, bool>> predicate,
            Expression<Func<T, TResult>> selector);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}
