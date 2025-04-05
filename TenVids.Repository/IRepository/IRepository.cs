
using System.Linq.Expressions;
using TenVids.Models;

namespace TenVids.Repository.IRepository
{
    public interface IRepository<T> where T : BaseEntity
    {
        void Add(T entity);
        void Update(T entity,T destination);
        void Remove(T item);
        void RemoveRange(IEnumerable<T> items);
        Task<bool> AnyAsync(Expression<Func<T, bool>> filter);
        Task<T> GetByIdAsync(int id, string includeProperties = null);
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, string? includeProperties = null);  
        Task <IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, string includeProperties = null,
            Func<IQueryable<T>,IOrderedQueryable<T>> orderby = null);
        Task<int> CountAsync(Expression<Func<T, bool>> filter = null);


        
    }
}
