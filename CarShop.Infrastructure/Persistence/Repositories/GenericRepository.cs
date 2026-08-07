using CarShop.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CarShop.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }


        public async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<TResult?> GetByIdAsync<TResult>(object id, Expression<Func<T, TResult>> selector)
        {
            return await _dbSet
                .Where(entity => EF.Property<object>(entity, "Id") == id) // Ensure you're querying by the id.
                .Select(selector)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<T, TResult>> selector)
        {
            return await _dbSet.Select(selector).ToListAsync();
        }

        public async Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector)
        {
            return await _dbSet.Where(predicate).Select(selector).ToListAsync();
        }

        public async Task<IEnumerable<TResult>> GetAllWithIncludesAsync<TResult>(Expression<Func<T, TResult>> selector,
            params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.Select(selector).ToListAsync();
        }

        public async Task<IEnumerable<TResult>> GetAllWithIncludesAsync<TResult>(Expression<Func<T, bool>> predicate,
            Expression<Func<T, TResult>> selector,
            params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.Where(predicate).Select(selector).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            return await _dbSet.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<IEnumerable<TResult>> GetPagedAsync<TResult>(int pageNumber, int pageSize, Expression<Func<T, TResult>> selector)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            return await _dbSet
                .Select(selector)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<TResult>> GetPagedAsync<TResult>(int pageNumber, int pageSize, Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            return await _dbSet.Where(predicate).Select(selector).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<IEnumerable<TResult>> GetPagedAsync<TResult>(int pageNumber, int pageSize, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> searchExpression = null!)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var allRecords = await _dbSet.ToListAsync();

            if (searchExpression != null)
            {
                allRecords = allRecords.Where(searchExpression.Compile()).ToList();
            }

            var projectedRecords = allRecords.Select(selector.Compile()).ToList();

            var pagedRecords = projectedRecords.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return pagedRecords;
        }

        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }

        public async Task<int> CountAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector)
        {
            return await _dbSet.Where(predicate).Select(selector).CountAsync();
        }

        public async Task<IEnumerable<T>> Where(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<TResult>> Where<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector)
        {
            return await _dbSet.Where(predicate).Select(selector).ToListAsync();
        }

        public async Task<IEnumerable<TResult>> GetDistinctAsync<TResult>(Expression<Func<T, TResult>> selector)
        {
            return await _dbSet.Select(selector).Distinct().ToListAsync();
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<TResult?> FirstOrDefaultAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector)
        {
            return await _dbSet.Where(predicate).Select(selector).FirstOrDefaultAsync();
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            foreach (var include in includes)
                query = query.Include(include);

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}
