using FacebookTimerPosts.AppDbContext;
using FacebookTimerPosts.Services.IRepository.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FacebookTimerPosts.Services.Repository.Base
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            T entity = await dbSet.FindAsync(id);
            await DeleteAsync(entity);
        }

        public async Task DeleteAsync(T entity)
        {
            dbSet.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await dbSet.AnyAsync(predicate);
        }

        public async Task<IList<T>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<IList<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await dbSet.Where(predicate).ToListAsync();
        }

        public async Task<IList<T>> GetAsync(Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }

            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, string includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _db.Entry(entity).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }
    }
}
