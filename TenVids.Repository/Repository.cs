using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TenVids.Data.Access.Data;
using TenVids.Repository.IRepository;

namespace TenVids.Repository
{
    public class Repository<T> : IRepository<T> where T : class 
    {

        private readonly TenVidsApplicationContext _context;  
        internal DbSet<T> _dbSet;    
        public Repository(TenVidsApplicationContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        {
         IQueryable<T> query = _dbSet;
            query.Where(filter);    
            return await query.AnyAsync();
        }

        public Task<int> Count(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = _dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return query.CountAsync();  

        }

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(
                    new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            if (orderby != null)
            {
                return await orderby(query).ToListAsync();
            }
        
            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int? id, string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(
                    new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            return await query.AsNoTracking()
                            .FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public async Task<T> GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<T> query = tracked ? _dbSet : _dbSet.AsNoTracking();

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(
                    new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            return await query.Where(filter).FirstOrDefaultAsync(); 
        }

        public void Remove(T item)
        {
            _context.Remove(item);
        }

        public void RemoveRange(IEnumerable<T> items)
        {
           _context.RemoveRange(items);
        }

        public void Update(T entity, T destination)
        {
           _context.Entry(entity).CurrentValues.SetValues(destination);
        }
    }
    
}
