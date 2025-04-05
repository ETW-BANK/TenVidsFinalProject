using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TenVids.Data.Access.Data;
using TenVids.Models;
using TenVids.Repository.IRepository;

namespace TenVids.Repository
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
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

        public async Task<int> CountAsync(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = _dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.CountAsync();  

        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }
              

            if (!string.IsNullOrEmpty(includeProperties))
            {
                query = GetWithProperties(query, includeProperties);    
            }

            if (orderby != null)
            {
                return await orderby(query).ToListAsync();
            }
        
            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id, string includeProperties = null)
        {
           IQueryable<T> query = _dbSet;

            if (!string.IsNullOrEmpty(includeProperties))
            {
               query = GetWithProperties(query, includeProperties);
            }

            return await query.Where(x => x.Id == id).FirstOrDefaultAsync();    
        }

        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;

            if (!string.IsNullOrEmpty(includeProperties))
            {
               query=GetWithProperties(query, includeProperties);
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

        #region Static Method
        public static IQueryable<T> GetWithProperties(IQueryable<T> query, string includeProperties)
        {
           var prop=includeProperties.Split(
                              new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            return query;
        }
        #endregion
    }



}


