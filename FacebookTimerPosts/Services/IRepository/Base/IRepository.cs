using System.Linq.Expressions;

namespace FacebookTimerPosts.Services.IRepository.Base
{
    public interface IRepository<T> where T : class
    {
        Task<IList<T>> GetAllAsync();
        Task<IList<T>> GetAsync(Expression<Func<T, bool>> predicate);
        Task<IList<T>> GetAsync(Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null);
        Task<T> GetByIdAsync(int id);
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, string includeProperties = null);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task DeleteAsync(T entity);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }
}
